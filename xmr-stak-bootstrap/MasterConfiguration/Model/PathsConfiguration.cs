using System.Runtime.Serialization;

namespace XmrStakBootstrap.MasterConfiguration.Model
{
    [DataContract]
    public class PathsConfiguration
    {
        [DataMember(Name = "config_template_path", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public string ConfigTemplate { get; set; }

        [DataMember(Name = "nvidia_template_path", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public string NvidiaTemplate { get; set; }

        [DataMember(Name = "cpu_template_path", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public string CpuTemplate { get; set; }

        [DataMember(Name = "amd_template_path", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public string AmdTemplate { get; set; }

        [DataMember(Name = "pools_template_path", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public string PoolsTemplate { get; set; }

        public PathsConfiguration()
        {
            ConfigTemplate = "config.source.txt";
            NvidiaTemplate = "nvidia.source.txt";
            CpuTemplate = "cpu.source.txt";
            AmdTemplate = "amd.source.txt";
            PoolsTemplate = "pools.source.txt";
        }
    }
}