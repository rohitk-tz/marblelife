using Core.Application.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
   public class EstimatePriceNotes : DomainBase
    {
        public string Notes { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public long? HoningmeasurementId { get; set; }
        [ForeignKey("HoningmeasurementId")]
        public virtual HoningMeasurement HoningMeasurement { get; set; }
    }
}
