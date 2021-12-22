using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Caviar.SharedKernel.Entities
{
    public partial class SysMenu : SysBaseEntity, IBaseEntity
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

        [StringLength(50, ErrorMessage = "LengthErrorMsg")]
        public string ControllerName { get; set; }
    }


}
