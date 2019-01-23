using System;
using System.Collections.Generic;
using System.Linq;
using TerraSketch.DataObjects.FieldObjects;
using TerraSketch.DataObjects.ParameterObjects;
using TerraSketch.FluentBuilders;
using TerraSketch.Generators;
using TerraSketch.Generators.Abstract;
using TerraSketch.Generators.LayerProfile;

namespace TerraSketch.Heightmap.Composer
{
    public class WorldDescriber : IWorldDescriber
    {
        private const int blurSize = 30;

        public IList<ILayerGlobalParameters> DescribeFields(IEnumerable<IField> fields)
        {
            var flds = fields.OrderBy(r => r.ZOrder);
            var desc = flds.Select(r => DescribeField(r)).ToList();
            return desc;
        }

        public ILayerGlobalParameters DescribeField(IField r)
        {

            var descriptor = setDescriptor(r);
            return descriptor;
        }

        private ILayerGlobalParameters setDescriptor(IField r)
        {

            var builder = ((ILayerProfile)r.Parameters.FieldProfile).GetBuilder();
            // HACK
            // the detail parameter should not affect noise parameters globally
            // for mountains,  high detail should have much different impact then on a flat terrain
            // which looks hideous when high details are applied.
            //var detail = (IDetailParameterEnhancer)r.Parameters.Detail;
            var detail = hack_GetDetail(builder, (IDetailParameterEnhancer)r.Parameters.Detail);
            //
            builder.Layer(l =>
                l.Polygon(r.Polygon)
                    .Blur(blurSize)
                    .ExtendSize(blurSize * 5)
            ).Noise(n =>
                n.ApplyDetailLevel(detail));

            var seed = r.Parameters.Seed ?? 987654321;
            var globalParameters = new LayerGlobalParameters
            {
                Generator = builder.Build(seed),
                BlendMode = r.Parameters.BlendModeWrap.BlendMode,
                Offset = r.Parameters.Offset
            };


            return globalParameters;
        }

        private IDetailParameterEnhancer hack_GetDetail(IGeneratorBuilder builder, IDetailParameterEnhancer detail)
        {
            if (builder.GetType().Name == "CombinerBuilder" &&  ((CombinerBuilder)builder).BuilderCount!=1)
                // in the current version it uniquely identifies the mountain builder
                return detail;

            switch (detail.GetType().Name)
            {
                case "LowDetail":
                    return new DetailEnhancer(1,4,.40f,2);
                case "MediumDetail":
                    return new DetailEnhancer(1, 5, .45f, 2);
                case "HighDetail":
                    return new DetailEnhancer(1, 6, .50f, 2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public class DetailEnhancer:IDetailParameterEnhancer

        {
            private readonly int _from;
            private readonly int _toOctave;
            private readonly float _ampl ;
            private  float baseAmpl =1;
            private readonly float _freq ;

            public DetailEnhancer(int fromOctave, int toOctave, float ampl, float freq)
            {
                _from = fromOctave;
                _toOctave = toOctave;
                _ampl = ampl;
                _freq = freq;
            }
            public void Enhance(INoiseParametersDetails np)
            {
                np.FromDepth = _from;
                np.ToDepth = _toOctave;
                np.Amplitude = _ampl;
                np.BaseAmplitude = baseAmpl;
                np.Lacunarity = _freq;
            }
        }

        public IBaseLayerDescriptor DescribeBaseLayer(IWorldInformativeParameters param, IField f)
        {

            var profile = new FlatProfile(
                new CombinerBuilder().Add(new HeightmapGeneratorBuilder().Influence(0.00005f)))
                ;
            BaseLayerDescriptor bs = new BaseLayerDescriptor(profile) { WorldParams = param };

            if (f != null)
            {
                var ds = setDescriptor(f);
                bs.LayerGlobalParameters = ds;
            }

            return bs;
        }
    }
}
