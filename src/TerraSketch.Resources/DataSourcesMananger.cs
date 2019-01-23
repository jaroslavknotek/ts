using System.Collections.Generic;
using TerraSketch.DataObjects.FieldObjects.FieldParams;
using TerraSketch.FluentBuilders;
using TerraSketch.Generators.LayerBlendMode;
using TerraSketch.Generators.LayerDetail;
using TerraSketch.Generators.LayerProfile;
using TerraSketch.Layer;

namespace TerraSketch.Resources
{

    public class DataSourcesMananger
    {
        public class DataSources
        {
            public IList<AFieldDetail> Details { get; private set; }
            public IList<AFieldProfile> Profiles { get; private set; }
            public IList<AFieldBlendMode> FieldBlendModes { get; private set; }

            public DataSources(IList<AFieldDetail> details, IList<AFieldProfile> profiles, IList<AFieldBlendMode> fieldBlendModes)
            {
                Details = details;
                Profiles = profiles;
                FieldBlendModes = fieldBlendModes;
            }
        }

        public DataSources GetDataSources()
        {
            var gatheredDetails = new List<AFieldDetail>()
            {
                new LowDetail(),
                new MediumDetail(),
                new HighDetail()
            };

            var gatheredFieldBlendModes = new List<AFieldBlendMode>()
            {
                new LayerBlendModeWrapItem("Replace", new SecondOnlyBlendMode()),
                new LayerBlendModeWrapItem("Add", new AddBlendMode()),
                new LayerBlendModeWrapItem("Subtract", new SubtractBlendMode()),
                new LayerBlendModeWrapItem("Min", new MinBlendMode()),
                new LayerBlendModeWrapItem("Max", new MaxBlendMode()),
                //new LayerBlendModeWrapItem("Screen", new Layer.BlendModes.ScreenBlendMode()),
                //new LayerBlendModeWrapItem("Overlay", new Layer.BlendModes.OverlayBlendMode()),
                new LayerBlendModeWrapItem("Invisible", new FirstOnlyBlendMode())

            };

            var gatheredProfiles = new List<AFieldProfile>()
            {
                // TODO use different generators
                new CanyonProfile(new CanyonGeneratorBuilder()),

                new FlatProfile(
                    new CombinerBuilder()
                    .Add(new HeightmapGeneratorBuilder().Influence(0.2f))),
            //new FlatProfile(new HeightmapGeneratorBuilder().Influence(0.0001f)),
                //new HillProfile(new HeightmapGeneratorBuilder()),
                new RockyMountainProfile(
                    new CombinerBuilder()
                    .Generator<MounatinGeneratorBuilder>(mgb=>
                        mgb.Influence(1.0f)
                    )
                    //.Generator<TriangularEdgeGeneratorBuilder>(mgb=>
                    //    mgb.Influence(1)
                    //)
                    .Generator<HeightmapGeneratorBuilder>(mgb=>
                        mgb.Influence(1)
                    )
                    )
            };

            return new DataSources(gatheredDetails,gatheredProfiles, gatheredFieldBlendModes);
        }

        
    }
}
