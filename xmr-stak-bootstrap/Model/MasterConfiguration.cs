using System.Runtime.Serialization;

namespace xmr_stak_bootstrap.Model
{
    [DataContract]
    public class MasterConfiguration
    {
        [DataMember(Name = "config_template_path", EmitDefaultValue = true, IsRequired = true, Order = 10)]
        public string ConfigTemplate { get; set; }

        [DataMember(Name = "nvidia_template_path", EmitDefaultValue = true, IsRequired = true, Order = 20)]
        public string NvidiaTemplate { get; set; }

        [DataMember(Name = "cpu_template_path", EmitDefaultValue = true, IsRequired = true, Order = 30)]
        public string CpuTemplate { get; set; }

        [DataMember(Name = "amd_template_path", EmitDefaultValue = true, IsRequired = true, Order = 40)]
        public string AmdTemplate { get; set; }

        [DataMember(Name = "pool_configuration", EmitDefaultValue = true, IsRequired = true, Order = 50)]
        public PoolConfiguration PoolConfiguration { get; set; }

        [DataMember(Name = "cpu_configuration", EmitDefaultValue = true, IsRequired = true, Order = 60)]
        public CpuConfiguration CpuConfiguration { get; set; }

        [DataMember(Name = "nvidia_configuration", EmitDefaultValue = true, IsRequired = true, Order = 70)]
        public NvidiaConfiguration NvidiaConfiguration { get; set; }

        [DataMember(Name = "amd_configuration", EmitDefaultValue = true, IsRequired = true, Order = 80)]
        public AmdConfiguration AmdConfiguration { get; set; }

        [DataMember(Name = "instance_configuration", EmitDefaultValue = true, IsRequired = true, Order = 90)]
        public InstanceConfiguration InstanceConfiguration { get; set; }

        [DataMember(Name = "solution_configuration", EmitDefaultValue = true, IsRequired = true, Order = 100)]
        public SolutionConfiguration SolutionConfiguration { get; set; }

        public MasterConfiguration()
        {
            ConfigTemplate = "config.source.txt";
            NvidiaTemplate = "nvidia.source.txt";
            CpuTemplate = "cpu.source.txt";
            AmdTemplate = "amd.source.txt";
        }
    }
}