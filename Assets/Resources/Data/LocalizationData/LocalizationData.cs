//----------------------------------------------
//    Google2u: Google Doc Unity integration
//         Copyright © 2015 Litteratus
//
//        This file has been auto-generated
//              Do not manually edit
//----------------------------------------------

using UnityEngine;
using System.Globalization;

namespace Google2u
{
	[System.Serializable]
	public class LocalizationDataRow : IGoogle2uRow
	{
		public string _Character;
		public string _Context;
		public string _English;
		public string _Arabic;
		public string _AudioFile;
		public LocalizationDataRow(string __STRING_ID, string __Character, string __Context, string __English, string __Arabic, string __AudioFile) 
		{
			_Character = __Character.Trim();
			_Context = __Context.Trim();
			_English = __English.Trim();
			_Arabic = __Arabic.Trim();
			_AudioFile = __AudioFile.Trim();
		}

		public int Length { get { return 5; } }

		public string this[int i]
		{
		    get
		    {
		        return GetStringDataByIndex(i);
		    }
		}

		public string GetStringDataByIndex( int index )
		{
			string ret = System.String.Empty;
			switch( index )
			{
				case 0:
					ret = _Character.ToString();
					break;
				case 1:
					ret = _Context.ToString();
					break;
				case 2:
					ret = _English.ToString();
					break;
				case 3:
					ret = _Arabic.ToString();
					break;
				case 4:
					ret = _AudioFile.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "Character":
					ret = _Character.ToString();
					break;
				case "Context":
					ret = _Context.ToString();
					break;
				case "English":
					ret = _English.ToString();
					break;
				case "Arabic":
					ret = _Arabic.ToString();
					break;
				case "AudioFile":
					ret = _AudioFile.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "Character" + " : " + _Character.ToString() + "} ";
			ret += "{" + "Context" + " : " + _Context.ToString() + "} ";
			ret += "{" + "English" + " : " + _English.ToString() + "} ";
			ret += "{" + "Arabic" + " : " + _Arabic.ToString() + "} ";
			ret += "{" + "AudioFile" + " : " + _AudioFile.ToString() + "} ";
			return ret;
		}
	}
	public sealed class LocalizationData : IGoogle2uDB
	{
		public enum rowIds {
			assessment_start_A1, assessment_start_A2, assessment_start_A3, assessment_result_intro, assessment_result_verygood, assessment_result_good, assessment_result_retry, match_words_drawings, end_game_A1, end_game_A2, end_game_A3, game_balloons_intro1, game_balloons_intro2, game_balloons_intro3, game_balloons_commentA, game_balloons_commentB, game_balloons_end, game_dontwake_attention, game_dontwake_intro1
			, game_dontwake_intro2, game_dontwake_intro3, game_dontwake_end, game_fastcrowd_A_intro1, game_fastcrowd_A_intro2, game_fastcrowd_A_end, game_fastcrowd_intro1, game_fastcrowd_intro2, game_fastcrowd_intro3, comment_catch_letters, comment_welldone, comment_great, map_A1, map_A2, map_A3, map_A4, map1_A1, map1_A2, assessment_intro_A1, assessment_intro_A2
			, assessment_intro_A3, assessment_intro_A4, mood_how_are_you_today, mood_how_do_you_feel, game_result_great_B, game_result_good_B, game_result_retry, game_result_fair, game_result_good, game_result_great, game_rewards_intro1, game_rewards_intro2, game_reward_A1, game_reward_A2, end_learningblock_A1, end_learningblock_A2, ui_living_letters, ui_game_title, wheel_A1, wheel_A2
			, wheel_A3, wheel_A4, wheel_A5, wheel_game_fastcrowd, wheel_turn, wheel_game_balloons_intro, wheel_game_balloons_end, wheel_game_fastcrowdA, wheel_locked, wheel_game_dontwake
		};
		public string [] rowNames = {
			"assessment_start_A1", "assessment_start_A2", "assessment_start_A3", "assessment_result_intro", "assessment_result_verygood", "assessment_result_good", "assessment_result_retry", "match_words_drawings", "end_game_A1", "end_game_A2", "end_game_A3", "game_balloons_intro1", "game_balloons_intro2", "game_balloons_intro3", "game_balloons_commentA", "game_balloons_commentB", "game_balloons_end", "game_dontwake_attention", "game_dontwake_intro1"
			, "game_dontwake_intro2", "game_dontwake_intro3", "game_dontwake_end", "game_fastcrowd_A_intro1", "game_fastcrowd_A_intro2", "game_fastcrowd_A_end", "game_fastcrowd_intro1", "game_fastcrowd_intro2", "game_fastcrowd_intro3", "comment_catch_letters", "comment_welldone", "comment_great", "map_A1", "map_A2", "map_A3", "map_A4", "map1_A1", "map1_A2", "assessment_intro_A1", "assessment_intro_A2"
			, "assessment_intro_A3", "assessment_intro_A4", "mood_how_are_you_today", "mood_how_do_you_feel", "game_result_great_B", "game_result_good_B", "game_result_retry", "game_result_fair", "game_result_good", "game_result_great", "game_rewards_intro1", "game_rewards_intro2", "game_reward_A1", "game_reward_A2", "end_learningblock_A1", "end_learningblock_A2", "ui_living_letters", "ui_game_title", "wheel_A1", "wheel_A2"
			, "wheel_A3", "wheel_A4", "wheel_A5", "wheel_game_fastcrowd", "wheel_turn", "wheel_game_balloons_intro", "wheel_game_balloons_end", "wheel_game_fastcrowdA", "wheel_locked", "wheel_game_dontwake"
		};
		public System.Collections.Generic.List<LocalizationDataRow> Rows = new System.Collections.Generic.List<LocalizationDataRow>();

		public static LocalizationData Instance
		{
			get { return NestedLocalizationData.instance; }
		}

		private class NestedLocalizationData
		{
			static NestedLocalizationData() { }
			internal static readonly LocalizationData instance = new LocalizationData();
		}

		private LocalizationData()
		{
			Rows.Add( new LocalizationDataRow("assessment_start_A1", "keeper", "assessment", "This evaluation is important, so listen carefully.", "هذا التقييم مهم جداً، انصت بعناية", ""));
			Rows.Add( new LocalizationDataRow("assessment_start_A2", "keeper", "assessment", "There are words and images on this screen.", "انظر الى هذه الكلمات والصور", ""));
			Rows.Add( new LocalizationDataRow("assessment_start_A3", "keeper", "assessment", "Draw lines so that each word has its right picture.", "صل الكلمة المناسبة بالرسم المناسب عن طريق رسم خط بينهما", ""));
			Rows.Add( new LocalizationDataRow("assessment_result_intro", "keeper", "assessment", "Let me have a look on that… hmmmm", "هممم، لنتمعَن سوياً في نتيجة التقييم", ""));
			Rows.Add( new LocalizationDataRow("assessment_result_verygood", "keeper", "assessment", "It’s fantastic! I’m really impressed.", "نتيجة ممتازة! انا منبهر", ""));
			Rows.Add( new LocalizationDataRow("assessment_result_good", "keeper", "assessment", "You did a very good job", "كان ادائك جيد جداً", ""));
			Rows.Add( new LocalizationDataRow("assessment_result_retry", "keeper", "assessment", "It’s promising, but we should explore this zone again", "محاولة لا بأس بها، لكن علينا استكشاف هذه المنطقة من جديد", ""));
			Rows.Add( new LocalizationDataRow("match_words_drawings", "keeper", "assessment", "Match the words with their images!", "طابق او صل كل كلمة مع صورتها المناسبة", ""));
			Rows.Add( new LocalizationDataRow("end_game_A1", "keeper", "playsession", "Thank you so much for your help today!", "شكراً جزيلاً على مساعدتك اليوم", ""));
			Rows.Add( new LocalizationDataRow("end_game_A2", "keeper", "playsession", "I hope you’ll come back soon to explore new regions with us.", "امل ان تعود قريباً لاستكشاف مناطق جديدة معنا", ""));
			Rows.Add( new LocalizationDataRow("end_game_A3", "keeper", "playsession", "Bye!", "الى اللقاء!", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_intro1", "keeper", "game balloons", "Pay attention on this one! ", "انتبه جيداً! ", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_intro2", "keeper", "game balloons", "Keep the good letters flying. ", "لا تفرقع بالونات الاحرف التي تراها في الكلمة.", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_intro3", "keeper", "game balloons", "Only explode the balloons of the wrong letters", "فرقع و بسرعة البالونات التي تحمل الحروف الغير موجودة في الكلمة.", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_commentA", "keeper", "game balloons", "Oh no!", "اه لا!", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_commentB", "keeper", "game balloons", "Outch !", "آخخخ!", ""));
			Rows.Add( new LocalizationDataRow("game_balloons_end", "keeper", "game balloons", "I hope everything goes well", "بالتوفيق", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_attention", "keeper", "game_dontwake", "Do not wake up Antura!", "لا توقظ عنتورة!", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro1", "keeper", "game_dontwake", "Shhhh! Everybody is sleeping…", "هشششش! الجميع نائم...", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro2", "keeper", "game_dontwake", "Try to move the image to the word without waking up anyone.", "حرك الصورة الى الكلمة دون ايقاظ احد", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro3", "keeper", "game_dontwake", "And especially, don't touch Antura", "خصوصاً عنتورة", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_end", "keeper", "game_dontwake", "Pfffff, it’s always the same", "اههه، ها نحن نعود من جديد", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_intro1", "keeper", "game_fastcrowd", "Oooh! There is Living Words wandering here!", "أنظر، بعض الحروف الحية تتجول هنالك  ", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_intro2", "keeper", "game_fastcrowd", "You must drag the right ones to the drop zone.", "عليك سحب الحروف المناسبة الى مربع الاسقاط ", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_end", "keeper", "game_fastcrowd", "Do we have enough of them", "هل لدينا العدد الكافي؟", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro1", "keeper", "game_fastcrowd", "Nice, look at these beautiful specimens!", "جميل، يا لها من مخلوقات رائعة!", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro2", "keeper", "game_fastcrowd", "Catch the right living letters and drag them to the capture zone.", "التقط الحروف المناسبة و اسحبها لمربع الاسقاط ", ""));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro3", "keeper", "game_fastcrowd", "Hurry up, we don’t have much time.", "اسرع! الوقت يداهمنا!", ""));
			Rows.Add( new LocalizationDataRow("comment_catch_letters", "keeper", "gameplay", "catch the letters", "التقط الحروف", ""));
			Rows.Add( new LocalizationDataRow("comment_welldone", "keeper", "gameplay", "well done", "أحسنت", ""));
			Rows.Add( new LocalizationDataRow("comment_great", "keeper", "gameplay", "great!", "ممتاز!", ""));
			Rows.Add( new LocalizationDataRow("map_A1", "keeper", "journey", "Hey you! Yes you!", "هاي انت! نعم انت!", ""));
			Rows.Add( new LocalizationDataRow("map_A2", "keeper", "journey", "We need your help right now!", "نحتاج لمساعدتك فوراً! نحتاج لمساعدتك حالاً؟", ""));
			Rows.Add( new LocalizationDataRow("map_A3", "keeper", "journey", "You are in the living letters territory. They are all around you", "انت في اراضي الحروف الحية! ستجدهم منتشرين في المكان.", ""));
			Rows.Add( new LocalizationDataRow("map_A4", "keeper", "journey", "Please try to catch some of them for us", "من فضلك، حاول اصطياد البعض منها. من فضلك، حاول التقاطها.", ""));
			Rows.Add( new LocalizationDataRow("map1_A1", "keeper", "journey", "You did a great job! We need to continue to explore this area.", "كان ادائك رائعاً! علينا متابعة استكشاف هذه المنطقة", ""));
			Rows.Add( new LocalizationDataRow("map1_A2", "keeper", "journey", "I’m sure we’ll find some more interesting specimen around here", "انا واثق  من انك ستجد مخلوقات اكثر غرابة و مثيرة للاعجاب", ""));
			Rows.Add( new LocalizationDataRow("assessment_intro_A1", "keeper", "journey", "Kid, You did a fantastic job during the last missions.", "ادائك كان عظيماً خلال المهمات السابقة", ""));
			Rows.Add( new LocalizationDataRow("assessment_intro_A2", "keeper", "journey", "But we are not playing anymore… I must evaluate you.", "علينا ايقاف اللعب الان. سوف نقوم بتقييم جدي", ""));
			Rows.Add( new LocalizationDataRow("assessment_intro_A3", "keeper", "journey", "Do you really know the species living in this region? We’ll see.", "لنرى كم انت ضليع في معرفة الحروف الحية في هذه المنطقة", ""));
			Rows.Add( new LocalizationDataRow("assessment_intro_A4", "keeper", "journey", "Be ready", "كن جاهزاً او تحضَر", ""));
			Rows.Add( new LocalizationDataRow("mood_how_are_you_today", "female", "mood", "How are you today?", "كيف حالك اليوم؟", ""));
			Rows.Add( new LocalizationDataRow("mood_how_do_you_feel", "female", "mood", "How do you feel now?", "ما هو شعورك الآن؟ كيف تشعر الآن؟", ""));
			Rows.Add( new LocalizationDataRow("game_result_great_B", "keeper", "results", "Very well", "جيد جداً", ""));
			Rows.Add( new LocalizationDataRow("game_result_good_B", "keeper", "results", "Good, but you can improve", "جيد، انا واثق من قدرتك على تحسين النتيجة", ""));
			Rows.Add( new LocalizationDataRow("game_result_retry", "keeper", "results", "Ahh.. i think you retry next time.", "لا بأس، يمكنك المحاولة لاحقاً", ""));
			Rows.Add( new LocalizationDataRow("game_result_fair", "keeper", "results", "Good work, we’ll get more next time", "جيد، سنحصل على المزيد في المرة المقبلة", ""));
			Rows.Add( new LocalizationDataRow("game_result_good", "keeper", "results", "That’s very well done", " نتيجة رائعة!", ""));
			Rows.Add( new LocalizationDataRow("game_result_great", "keeper", "results", "You did a perfect job!", "عمل ممتاز!", ""));
			Rows.Add( new LocalizationDataRow("game_rewards_intro1", "keeper", "results", "It’s the end of this mission. And look at that!", "انها نهاية المرحلة. انظر الى هذا!", ""));
			Rows.Add( new LocalizationDataRow("game_rewards_intro2", "keeper", "results", "It’s seems you win something to customize Antura", "لقد ربحت شيئاً رائعاً، انها تخصيصات لتعديل شكل عنتورة", ""));
			Rows.Add( new LocalizationDataRow("game_reward_A1", "keeper", "results", "Congratulation.", "مبروك", ""));
			Rows.Add( new LocalizationDataRow("game_reward_A2", "keeper", "results", "I’m sure Antura will love it", "انا واثق ان عنتورة سيحب هذه الاضافات", ""));
			Rows.Add( new LocalizationDataRow("end_learningblock_A1", "keeper", "results", "You did a lot of effort to contribute to the Living Letters collection", "قمت بمجهود عظيم خلال مساهمتك بالتقاط الحروف الحية", ""));
			Rows.Add( new LocalizationDataRow("end_learningblock_A2", "keeper", "results", "I have a little gift for you", "لدي هدية مميزة لك!", ""));
			Rows.Add( new LocalizationDataRow("ui_living_letters", "kids or female", "ui", "Living Letters", "الحروف الحية", ""));
			Rows.Add( new LocalizationDataRow("ui_game_title", "kids or female", "ui", "Antura and the letters!", "عنتورة و الحروف", ""));
			Rows.Add( new LocalizationDataRow("wheel_A1", "keeper", "wheel", "My dog Antura will help you to find the letters.", "كلبي عنتورة سيساعدك في العثور على الحروف ", ""));
			Rows.Add( new LocalizationDataRow("wheel_A2", "keeper", "wheel", "but he’s not very focus on his job.", "لكن التركيز ليس من افضل سماته خلال العمل", ""));
			Rows.Add( new LocalizationDataRow("wheel_A3", "keeper", "wheel", "Well!", "حسناً!", ""));
			Rows.Add( new LocalizationDataRow("wheel_A4", "keeper", "wheel", "This is the game wheel, we use it to attract the living letters. They loooove to play.", "هذه لعبة الدولاب، نستعملها لجذب انتباه الحروف. انها تعشششق اللعب", ""));
			Rows.Add( new LocalizationDataRow("wheel_A5", "keeper", "wheel", "Come on turn the wheel!", "هيا بنا، ادر الدولاب!", ""));
			Rows.Add( new LocalizationDataRow("wheel_game_fastcrowd", "keeper", "wheel", "Ah ah! It’s the game Fast crowd! Let see what we can catch with it", "اااه! لعبة جماهير الحروف! لنرى ماذا سنصطاد اليوم؟", ""));
			Rows.Add( new LocalizationDataRow("wheel_turn", "keeper", "wheel", "turn the wheel", "ادر الدولاب", ""));
			Rows.Add( new LocalizationDataRow("wheel_game_balloons_intro", "keeper", "wheel", "Time to catch some more living letters!", "حان الوقت لالتقاط مزيداً من الحروف!", ""));
			Rows.Add( new LocalizationDataRow("wheel_game_balloons_end", "keeper", "wheel", "The ballooooon. You gonna catch many of them with it", "لعبة البالونات. سوف تصطاد فيها الكثير من الحروف", ""));
			Rows.Add( new LocalizationDataRow("wheel_game_fastcrowdA", "keeper", "wheel", "Hummmm, interesting.", "همممم، مشوق.", ""));
			Rows.Add( new LocalizationDataRow("wheel_locked", "keeper", "wheel", "Ok", "حسناً", ""));
			Rows.Add( new LocalizationDataRow("wheel_game_dontwake", "keeper", "wheel", "Where is Antura? Oh no, he felt asleep among the Living Words specimens…", "اين هو عنتورة؟ اهه كلا، انه يأخذ قيلولة وسط منطقة الحروف الحية", ""));
		}
		public IGoogle2uRow GetGenRow(string in_RowString)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}
		public IGoogle2uRow GetGenRow(rowIds in_RowID)
		{
			IGoogle2uRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LocalizationDataRow GetRow(rowIds in_RowID)
		{
			LocalizationDataRow ret = null;
			try
			{
				ret = Rows[(int)in_RowID];
			}
			catch( System.Collections.Generic.KeyNotFoundException ex )
			{
				Debug.LogError( in_RowID + " not found: " + ex.Message );
			}
			return ret;
		}
		public LocalizationDataRow GetRow(string in_RowString)
		{
			LocalizationDataRow ret = null;
			try
			{
				ret = Rows[(int)System.Enum.Parse(typeof(rowIds), in_RowString)];
			}
			catch(System.ArgumentException) {
				Debug.LogError( in_RowString + " is not a member of the rowIds enumeration.");
			}
			return ret;
		}

	}

}
