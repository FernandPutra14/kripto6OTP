using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desainUIKripto
{
    public class KunciTerpakai
    {
        [Key]
        public int Id { get; set; }

        public string Key { get; set; }

        public DateTime WaktuDigunakan { get; set; }
    }
}
