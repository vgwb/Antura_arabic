using System.Collections.Generic;

namespace EA4S.Database.Management
{
    /// <summary>
    /// Custom JSON parser for MiniGameData
    /// </summary>
    public class MiniGameParser : DataParser<MiniGameData, MiniGameTable>
    {
        override protected MiniGameData CreateData(Dictionary<string, object> dict, DatabaseObject db)
        {
            var data = new MiniGameData();

            data.Code = ParseEnum<MiniGameCode>(data, dict["Id"]);
            data.Main = ToString(dict["Main"]);
            data.Variation = ToString(dict["Variation"]);
            data.Type = ParseEnum<MiniGameDataType>(data, dict["Type"]);
            data.Title_En = ToString(dict["Title_En"]);
            data.Title_Ar = ToString(dict["Title_Ar"]);
            data.Scene = ToString(dict["Scene"]);
            data.Available = ToString(dict["Status"]) == "active";
            data.PlaySkills = CustomParsePlaySkills(data, dict);

            return data;
        }

        protected override void RegenerateEnums(List<Dictionary<string, object>> rowdicts_list)
        {
            // TODO: should we generate also the MiniGameCode? Could be useful, but it could mess with the current inspector values. 
            // ExtractEnum(rowdicts_list, "Id", customEnumName: "MiniGameCode");
            ExtractEnum(rowdicts_list, "Type");
        }


        List<MiniGameSkill> CustomParsePlaySkills(MiniGameData data, Dictionary<string, object> dict)
        {
            var list = new List<MiniGameSkill>();

            // string[] skillFileds = { "SkillTiming", "SkillPrecision", "SkillObservation", "SkillListening", "SkillLogic", "SkillMemory" };

            if (ToString(dict["SkillTiming"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Timing, ToFloat(dict["SkillTiming"]));
                list.Add(skill);
            }

            if (ToString(dict["SkillPrecision"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Precision, ToFloat(dict["SkillPrecision"]));
                list.Add(skill);
            }

            if (ToString(dict["SkillObservation"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Observation, ToFloat(dict["SkillObservation"]));
                list.Add(skill);
            }

            if (ToString(dict["SkillListening"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Listening, ToFloat(dict["SkillListening"]));
                list.Add(skill);
            }

            if (ToString(dict["SkillLogic"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Logic, ToFloat(dict["SkillLogic"]));
                list.Add(skill);
            }

            if (ToString(dict["SkillMemory"]) != "") {
                var skill = new MiniGameSkill(PlaySkill.Memory, ToFloat(dict["SkillMemory"]));
                list.Add(skill);
            }

            return list;
        }
    }
}
