using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraSketch.DataObjects.FieldObjects.FieldParams;

namespace TerraSketch.DataObjects.SaveLoad
{
    public interface ILoadItemParameter
    {

        
        IList<AFieldDetail> Details
        {
            get;
        }
        
        IList<AFieldProfile> Profiles
        {
            get;
        }
        IList<AFieldBlendMode> BlendModes
        {
            get;
        }
    }
    public class LoadItemsParameter : ILoadItemParameter
    {
       
        public IList<AFieldDetail> Details
        {
            get; set;
        }
        
        public IList<AFieldProfile> Profiles
        {
            get; set;
        }
        public IList<AFieldBlendMode> BlendModes
        {
            get;
            set;
        }




    }
}
