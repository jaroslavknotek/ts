import matplotlib.pyplot as plt
from perlin_noise import PerlinNoise
import numpy as np
from scipy.spatial import Voronoi, voronoi_plot_2d
import random
import math
random.seed(123)
np.random.seed(123)
import matplotlib
matplotlib.rcParams['figure.figsize'] = [10, 10]


import numpy as np
import scipy as sp
import matplotlib.pyplot as plt
import os
import sys
import util


# Smooths out slopes of `terrain` that are too steep. Rough approximation of the
# phenomenon described here: https://en.wikipedia.org/wiki/Angle_of_repose
def apply_slippage(terrain, repose_slope, cell_width):
  delta = util.simple_gradient(terrain) / cell_width
  smoothed = util.gaussian_blur(terrain, sigma=1.5)
  should_smooth = np.abs(delta) > repose_slope
  result = np.select([np.abs(delta) > repose_slope], [smoothed], terrain)
  return result


def main(argv):
  # Grid dimension constants
  full_width = 200
  dim = 512
  shape = [dim] * 2
  cell_width = full_width / dim
  cell_area = cell_width ** 2

  # Snapshotting parameters. Only needed for generating the simulation
  # timelapse.
  enable_snapshotting = False
  my_dir = os.path.dirname(argv[0])
  snapshot_dir = os.path.join(my_dir, 'sim_snaps')
  snapshot_file_template = 'sim-%05d.png'
  if enable_snapshotting:
    try: os.mkdir(snapshot_dir)
    except: pass

  # Water-related constants
  rain_rate = 0.0008 * cell_area
  evaporation_rate = 0.0005

  # Slope constants
  min_height_delta = 0.05
  repose_slope = 0.03
  gravity = 30.0
  gradient_sigma = 0.5

  # Sediment constants
  sediment_capacity_constant = 50.0
  dissolving_rate = 0.25
  deposition_rate = 0.001

  # The numer of iterations is proportional to the grid dimension. This is to 
  # allow changes on one side of the grid to affect the other side.
  iterations = int(1.4 * dim)

  # `terrain` represents the actual terrain height we're interested in
  terrain = util.fbm(shape, -2.0)

  # `sediment` is the amount of suspended "dirt" in the water. Terrain will be
  # transfered to/from sediment depending on a number of different factors.
  sediment = np.zeros_like(terrain)

  # The amount of water. Responsible for carrying sediment.
  water = np.zeros_like(terrain)

  # The water velocity.
  velocity = np.zeros_like(terrain)

  for i in range(0, iterations):
    print('%d / %d' % (i + 1, iterations))

    # Add precipitation. This is done by via simple uniform random distribution,
    # although other models use a raindrop model
    water += np.random.rand(*shape) * rain_rate

    # Compute the normalized gradient of the terrain height to determine where 
    # water and sediment will be moving.
    gradient = np.zeros_like(terrain, dtype='complex')
    gradient = util.simple_gradient(terrain)
    gradient = np.select([np.abs(gradient) < 1e-10],
                             [np.exp(2j * np.pi * np.random.rand(*shape))],
                             gradient)
    gradient /= np.abs(gradient)

    # Compute the difference between teh current height the height offset by
    # `gradient`.
    neighbor_height = util.sample(terrain, -gradient)
    height_delta = terrain - neighbor_height
    
    # The sediment capacity represents how much sediment can be suspended in
    # water. If the sediment exceeds the quantity, then it is deposited,
    # otherwise terrain is eroded.
    sediment_capacity = (
        (np.maximum(height_delta, min_height_delta) / cell_width) * velocity *
        water * sediment_capacity_constant)
    deposited_sediment = np.select(
        [
          height_delta < 0, 
          sediment > sediment_capacity,
        ], [
          np.minimum(height_delta, sediment),
          deposition_rate * (sediment - sediment_capacity),
        ],
        # If sediment <= sediment_capacity
        dissolving_rate * (sediment - sediment_capacity))

    # Don't erode more sediment than the current terrain height.
    deposited_sediment = np.maximum(-height_delta, deposited_sediment)

    # Update terrain and sediment quantities.
    sediment -= deposited_sediment
    terrain += deposited_sediment
    sediment = util.displace(sediment, gradient)
    water = util.displace(water, gradient)

    # Smooth out steep slopes.
    terrain = apply_slippage(terrain, repose_slope, cell_width)

    # Update velocity
    velocity = gravity * height_delta / cell_width
  
    # Apply evaporation
    water *= 1 - evaporation_rate

    # Snapshot, if applicable.
    if enable_snapshotting:
      output_path = os.path.join(snapshot_dir, snapshot_file_template % i)
      util.save_as_png(terrain, output_path)


  np.save('simulation', util.normalize(terrain))

  

def compute_voronoi_regions(points_count):
    
    ps = [ random.random() for _ in range(points_count *2)]
    points = np.array(ps).reshape((points_count,2))

    vor = Voronoi(points)
    # fig = voronoi_plot_2d(vor)
    return [ ( [vor.vertices[v_index] for v_index in region if v_index>=0 ]) for region in vor.regions if len(region) > 0]

def interpolant(t):
    return t*t*t*(t*(t*6 - 15) + 10)


def generate_perlin_noise_2d(
        shape, res, tileable=(False, False), interpolant=interpolant
):
    """Generate a 2D numpy array of perlin noise.
    Args:
        shape: The shape of the generated array (tuple of two ints).
            This must be a multple of res.
        res: The number of periods of noise to generate along each
            axis (tuple of two ints). Note shape must be a multiple of
            res.
        tileable: If the noise should be tileable along each axis
            (tuple of two bools). Defaults to (False, False).
        interpolant: The interpolation function, defaults to
            t*t*t*(t*(t*6 - 15) + 10).
    Returns:
        A numpy array of shape shape with the generated noise.
    Raises:
        ValueError: If shape is not a multiple of res.
    """
    delta = (res[0] / shape[0], res[1] / shape[1])
    d = (shape[0] // res[0], shape[1] // res[1])
    grid = np.mgrid[0:res[0]:delta[0], 0:res[1]:delta[1]]\
             .transpose(1, 2, 0) % 1
    # Gradients
    angles = 2*np.pi*np.random.rand(res[0]+1, res[1]+1)
    gradients = np.dstack((np.cos(angles), np.sin(angles)))
    if tileable[0]:
        gradients[-1,:] = gradients[0,:]
    if tileable[1]:
        gradients[:,-1] = gradients[:,0]
    gradients = gradients.repeat(d[0], 0).repeat(d[1], 1)
    g00 = gradients[    :-d[0],    :-d[1]]
    g10 = gradients[d[0]:     ,    :-d[1]]
    g01 = gradients[    :-d[0],d[1]:     ]
    g11 = gradients[d[0]:     ,d[1]:     ]
    # Ramps
    n00 = np.sum(np.dstack((grid[:,:,0]  , grid[:,:,1]  )) * g00, 2)
    n10 = np.sum(np.dstack((grid[:,:,0]-1, grid[:,:,1]  )) * g10, 2)
    n01 = np.sum(np.dstack((grid[:,:,0]  , grid[:,:,1]-1)) * g01, 2)
    n11 = np.sum(np.dstack((grid[:,:,0]-1, grid[:,:,1]-1)) * g11, 2)
    # Interpolation
    t = interpolant(grid)
    n0 = n00*(1-t[:,:,0]) + t[:,:,0]*n10
    n1 = n01*(1-t[:,:,0]) + t[:,:,0]*n11
    return np.sqrt(2)*((1-t[:,:,1])*n0 + t[:,:,1]*n1)


def noise(res_x,res_y, start_octave = 1, end_octave=4):
    noise = np.zeros((res_x,res_y))
    for i in range(start_octave,end_octave):
        octave = int(math.pow(2,i))
        perlin = generate_perlin_noise_2d((res_x,res_y),(octave,octave))
        noise += perlin*math.pow(2,-i)
    
    return noise

def print_texture(texture):
    size = len(texture)
    xy = np.array([ (xx,yy) for xx in range(size) for yy in range(size)])
    x = xy[:,0]
    y = xy[:,1]
    z = texture.reshape((size*size,))


    plt.imshow(texture, cmap='gray')
    plt.show()

    ax = plt.axes(projection='3d')
    ax.plot_trisurf(x, y, z,                cmap='viridis', edgecolor='none');


p_count = 20
# compute_voronoi_regions(p_count)
size=64
texture = noise(size,size,end_octave =5)

print_texture(texture)
