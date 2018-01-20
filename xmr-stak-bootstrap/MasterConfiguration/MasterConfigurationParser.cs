using System.IO;
using Newtonsoft.Json;
using Unity.Attributes;
using XmrStakBootstrap.MasterConfiguration.Model;
using XmrStakBootstrap.RunConfiguration.Model;

namespace XmrStakBootstrap.MasterConfiguration
{
    public class MasterConfigurationParser : IMasterConfigurationParser
    {
        [Dependency]
        public RunConfigurationModel ConfigurationModel { get; set; }

        public MasterConfigurationModel Parse()
        {
            if (!File.Exists(ConfigurationModel.MasterConfiguration))
            {
                return null;
            }

            var contents = File.ReadAllText(ConfigurationModel.MasterConfiguration);
            return JsonConvert.DeserializeObject<MasterConfigurationModel>(contents);
        }
    }
}