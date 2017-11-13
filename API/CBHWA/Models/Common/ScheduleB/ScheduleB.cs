using System;

namespace CBHWA.Models
{
    public class ScheduleB
    {
        public string SchBNum { get; set; }
        public string SchBShortDescription { get; set; }
        public string SchBLongDescription { get; set; }
        public string SchBUnitOfMeasure { get; set; }
        public string SchBUnitOfMeasure2 { get; set; }
        public string SchBSITC { get; set; }
        public string SchBEndUseClassification { get; set; }
        public bool SchBUSDA { get; set; }
        public string SchBNAICS { get; set; }
        public string SchBHiTechClassicification { get; set; }
        public bool SchBImport { get; set; }
        public bool SchBExport { get; set; }
        public bool SchBRetired { get; set; }
        public string SchBModifiedBy { get; set; }
        public Nullable<DateTime> SchBModifiedDate { get; set; }
        public string SchBCreatedBy { get; set; }
        public DateTime SchBCreatedDate { get; set; }
    }

    public class SBLanguage
    {
        public int SBLanguageKey { get; set; }
        public string SBLanguageSchBNum { get; set; }
        public string SBLanguageSchBSubNum { get; set; }
        public string SBLanguageCode { get; set; }
        public string SBLanguageText { get; set; }

        public string x_Language { get; set; }
    }
}