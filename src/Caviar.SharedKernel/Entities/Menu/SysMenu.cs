using System.ComponentModel.DataAnnotations;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysMenu : SysUseEntity
    {

        [Required(ErrorMessage = "RequiredErrorMsg")]
        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Key { get; set; }

        public MenuType MenuType { get; set; }

        public TargetType TargetType { get; set; }

        [StringLength(1024, ErrorMessage = "LengthErrorMsg")]
        public string Url { get; set; }

        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string Icon { get; set; }

        public int ParentId { get; set; }

        public ButtonPosition ButtonPosition { get; set; }

        public bool IsDoubleTrue { get; set; }

        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string HttpMethods { get; set; }
    }


}
