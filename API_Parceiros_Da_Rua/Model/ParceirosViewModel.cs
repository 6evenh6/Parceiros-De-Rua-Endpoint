using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_Parceiros_Da_Rua.Model
{
    public class ParceirosViewModel
    {
        // idparceiros, parceirosnome, parceirosidade, parceirossexo, parceiroscpf, parceirosdatenow
        [JsonIgnore]
        public int idparceiros { get; set; }


        [Required(ErrorMessage = "Nome do parceiro obrigatorio.")]
        public string? parceirosnome { get; set; }


        [Required(ErrorMessage = "Idade do parceiro obrigatorio.")]
        public int parceirosidade { get; set; }


        [Required(ErrorMessage = "Sexo do parceiro obrigatorio.")]
        public string? parceirossexo { get; set; }
        public double parceiroscpf { get; set; }
        public DateTime parceirosdatenow { get; set; } = DateTime.Now;
    }
}
