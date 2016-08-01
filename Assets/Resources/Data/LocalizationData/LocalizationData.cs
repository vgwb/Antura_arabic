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
		public string _EnglishOld;
		public LocalizationDataRow(string __STRING_ID, string __Character, string __Context, string __English, string __Arabic, string __AudioFile, string __EnglishOld) 
		{
			_Character = __Character.Trim();
			_Context = __Context.Trim();
			_English = __English.Trim();
			_Arabic = __Arabic.Trim();
			_AudioFile = __AudioFile.Trim();
			_EnglishOld = __EnglishOld.Trim();
		}

		public int Length { get { return 6; } }

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
				case 5:
					ret = _EnglishOld.ToString();
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
				case "EnglishOld":
					ret = _EnglishOld.ToString();
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
			ret += "{" + "EnglishOld" + " : " + _EnglishOld.ToString() + "} ";
			return ret;
		}
	}
	public sealed class LocalizationData : IGoogle2uDB
	{
		public enum rowIds {
			assessment_start_A1, assessment_start_A2, assessment_start_A3, assessment_result_intro, assessment_result_verygood, assessment_result_good, assessment_result_retry, match_words_drawings, game_balloons_intro1, game_balloons_intro2, game_balloons_intro3, game_balloons_commentA, game_balloons_commentB, game_balloons_end, game_dontwake_attention, game_dontwake_intro1, game_dontwake_intro2, game_dontwake_intro3, game_dontwake_end
			, game_fastcrowd_A_intro1, game_fastcrowd_A_intro2, game_fastcrowd_A_end, game_fastcrowd_intro1, game_fastcrowd_intro2, game_fastcrowd_intro3, comment_catch_letters, comment_welldone, comment_great, map_A1, map_A2, map_A3, map_A4, map1_A1, map1_A2, assessment_intro_A1, assessment_intro_A2, assessment_intro_A3, assessment_intro_A4, mood_how_are_you_today
			, mood_how_do_you_feel, end_game_A1, end_game_A2, end_game_A3, game_result_great_B, game_result_good_B, game_result_retry, game_result_fair, game_result_good, game_result_great, game_rewards_intro1, game_rewards_intro2, game_reward_A1, game_reward_A2, end_learningblock_A1, end_learningblock_A2, ui_living_letters, ui_game_title, wheel_A1, wheel_A2
			, wheel_A3, wheel_A4, wheel_A5, wheel_game_fastcrowd, wheel_turn, wheel_game_balloons_intro, wheel_game_balloons_end, wheel_game_fastcrowdA, wheel_locked, wheel_game_dontwake, game_dontwake_fail_antura, game_dontwake_fail_alarms, game_dontwake_fail_toofast, game_dontwake_fail_fall
		};
		public string [] rowNames = {
			"assessment_start_A1", "assessment_start_A2", "assessment_start_A3", "assessment_result_intro", "assessment_result_verygood", "assessment_result_good", "assessment_result_retry", "match_words_drawings", "game_balloons_intro1", "game_balloons_intro2", "game_balloons_intro3", "game_balloons_commentA", "game_balloons_commentB", "game_balloons_end", "game_dontwake_attention", "game_dontwake_intro1", "game_dontwake_intro2", "game_dontwake_intro3", "game_dontwake_end"
			, "game_fastcrowd_A_intro1", "game_fastcrowd_A_intro2", "game_fastcrowd_A_end", "game_fastcrowd_intro1", "game_fastcrowd_intro2", "game_fastcrowd_intro3", "comment_catch_letters", "comment_welldone", "comment_great", "map_A1", "map_A2", "map_A3", "map_A4", "map1_A1", "map1_A2", "assessment_intro_A1", "assessment_intro_A2", "assessment_intro_A3", "assessment_intro_A4", "mood_how_are_you_today"
			, "mood_how_do_you_feel", "end_game_A1", "end_game_A2", "end_game_A3", "game_result_great_B", "game_result_good_B", "game_result_retry", "game_result_fair", "game_result_good", "game_result_great", "game_rewards_intro1", "game_rewards_intro2", "game_reward_A1", "game_reward_A2", "end_learningblock_A1", "end_learningblock_A2", "ui_living_letters", "ui_game_title", "wheel_A1", "wheel_A2"
			, "wheel_A3", "wheel_A4", "wheel_A5", "wheel_game_fastcrowd", "wheel_turn", "wheel_game_balloons_intro", "wheel_game_balloons_end", "wheel_game_fastcrowdA", "wheel_locked", "wheel_game_dontwake", "game_dontwake_fail_antura", "game_dontwake_fail_alarms", "game_dontwake_fail_toofast", "game_dontwake_fail_fall"
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
			Rows.Add( new LocalizationDataRow("assessment_start_A1", "keeper", "assessment", "This evaluation is so important, listen carefully. ", "هذا التقييم مهم جداً، انصت بعناية", "assessment_start_A1", "This evaluation is important, so listen carefully."));
			Rows.Add( new LocalizationDataRow("assessment_start_A2", "keeper", "assessment", "Look at the following words and pictures. ", "انظر الى هذه الكلمات والصور", "assessment_start_A2", "There are words and images on this screen."));
			Rows.Add( new LocalizationDataRow("assessment_start_A3", "keeper", "assessment", "Using lines, connect the correct word with its picture. ", "صل الكلمة المناسبة بالرسم المناسب عن طريق رسم خط بينهما", "assessment_start_A3", "Draw lines so that each word has its right picture."));
			Rows.Add( new LocalizationDataRow("assessment_result_intro", "keeper", "assessment", "Hmm, Let's look at the evaluation result. ", "هممم، لنتمعَن سوياً في نتيجة التقييم", "assessment_result_intro", "Let me have a look on that… hmmmm"));
			Rows.Add( new LocalizationDataRow("assessment_result_verygood", "keeper", "assessment", "Great result! I am really impressed. ", "نتيجة ممتازة! انا منبهر", "assessment_result_verygood", "It’s fantastic! I’m really impressed."));
			Rows.Add( new LocalizationDataRow("assessment_result_good", "keeper", "assessment", "Your performance was excellent! ", "كان ادائك جيد جداً", "assessment_result_good", "You did a very good job"));
			Rows.Add( new LocalizationDataRow("assessment_result_retry", "keeper", "assessment", "Acceptable try, but we have to investigate this area. ", "محاولة لا بأس بها، لكن علينا استكشاف هذه المنطقة من جديد", "assessment_result_retry", "It’s promising, but we should explore this zone again"));
			Rows.Add( new LocalizationDataRow("match_words_drawings", "keeper", "assessment", "Match the correct words with their images. ", "طابق كل كلمة مع صورتها المناسبة", "match_words_drawings", "Match the words with their images!"));
			Rows.Add( new LocalizationDataRow("game_balloons_intro1", "keeper", "game balloons", "Pay attention carefully! ", "انتبه جيداً! ", "game_balloons_intro1", "Pay attention to this one! "));
			Rows.Add( new LocalizationDataRow("game_balloons_intro2", "keeper", "game balloons", "Do not pop the letter balloons. ", "لا تفرقع بالونات الأحرف التي تراها في الكلمة", "game_balloons_intro2", "Keep the good letters flying. "));
			Rows.Add( new LocalizationDataRow("game_balloons_intro3", "keeper", "game balloons", "Pop only the balloons of the letters that are unavailable in the word quickly. ", "فرقع البالونات التي تحمل الحروف الغير موجودة في الكلمة و بسرعة.", "game_balloons_intro3", "Only pop the balloons of the wrong letters "));
			Rows.Add( new LocalizationDataRow("game_balloons_commentA", "keeper", "game balloons", "Oh no! ", "اه لا!", "game_balloons_commentA", "Oh no!"));
			Rows.Add( new LocalizationDataRow("game_balloons_commentB", "keeper", "game balloons", "Ouch!", "آخخخ!", "game_balloons_commentB", "Outch !"));
			Rows.Add( new LocalizationDataRow("game_balloons_end", "keeper", "game balloons", "All the best. ", "بالتوفيق", "game_balloons_end", "I hope everything goes well"));
			Rows.Add( new LocalizationDataRow("game_dontwake_attention", "keeper", "game_dontwake", "Do not wake up Antura! ", "لا توقظ عنتورة!", "game_dontwake_attention", "Do not wake up Antura!"));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro1", "keeper", "game_dontwake", "Shhh! All are asleep. ", "شششششششش! الجميع نائم...", "game_dontwake_intro1", "Shhhh! Everybody is sleeping…"));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro2", "keeper", "game_dontwake", "Move the picture toward the word without waking up anyone. ", "حرك الصورة نحو الكلمة دون ايقاظ احد", "game_dontwake_intro2", "Try to move the image towards the word without waking up anyone."));
			Rows.Add( new LocalizationDataRow("game_dontwake_intro3", "keeper", "game_dontwake", "especially Antura. ", "خصوصاً عنتورة", "game_dontwake_intro3", "And especially, don't touch Antura"));
			Rows.Add( new LocalizationDataRow("game_dontwake_end", "keeper", "game_dontwake", "Aha, here we go again. ", "اههه، ها نحن نعود من جديد", "game_dontwake_end", "Pfffff, it’s always the same"));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_intro1", "keeper", "game_fastcrowd", "Look! There are some living words wandering here. ", "أنظر، بعض الحروف الحية تتجول هنا.   ", "game_fastcrowd_A_intro1", "Oooh! There are Living Words wandering here!"));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_intro2", "keeper", "game_fastcrowd", "You have to drag the correct words into the dropping zone. ", "عليك سحب الحروف المناسبة الى مربع الاسقاط ", "game_fastcrowd_A_intro2", "You must drag the right ones to the drop zone."));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_A_end", "keeper", "game_fastcrowd", "Do we have enough?", "هل لدينا العدد الكافي؟", "game_fastcrowd_A_end", "Do we have enough of them? "));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro1", "keeper", "game_fastcrowd", "Wow! Look at these wonderful creatures. ", "جميل، يا لها من مخلوقات رائعة!", "game_fastcrowd_intro1", "Nice, look at these beautiful specimens!"));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro2", "keeper", "game_fastcrowd", "Catch the correct living letters and drag them into the dropping zone. ", "التقط الحروف المناسبة و اسحبهم إلى مربع الاسقاط ", "game_fastcrowd_intro2", "Catch the right living letters and drag them to the drop zone."));
			Rows.Add( new LocalizationDataRow("game_fastcrowd_intro3", "keeper", "game_fastcrowd", "Hurry up! We are running out of time. ", "اسرع! الوقت يداهمنا!", "game_fastcrowd_intro3", "Hurry up, we don’t have much time."));
			Rows.Add( new LocalizationDataRow("comment_catch_letters", "keeper", "gameplay", "Pick up the letters. ", "التقط الحروف", "comment_catch_letters", "catch the letters"));
			Rows.Add( new LocalizationDataRow("comment_welldone", "keeper", "gameplay", "Well done! ", "أحسنت", "comment_welldone", "well done"));
			Rows.Add( new LocalizationDataRow("comment_great", "keeper", "gameplay", "Great job!", "ممتاز!", "comment_great", "great!"));
			Rows.Add( new LocalizationDataRow("map_A1", "keeper", "journey", "Hey you! Yes you! ", "هاي انت! نعم انت!", "map_A1", "Hey you! Yes you!"));
			Rows.Add( new LocalizationDataRow("map_A2", "keeper", "journey", "We need your help right now. ", "نحتاج لمساعدتك فوراً! نحتاج لمساعدتك حالاً؟", "map_A2", "We need your help right now!"));
			Rows.Add( new LocalizationDataRow("map_A3", "keeper", "journey", "You are in the living letters' territory. They are surrounding you. ", "انت في اراضي الحروف الحية! ستجدهم منتشرين في المكان.", "map_A3", "You are in the living letters territory. They are all around you"));
			Rows.Add( new LocalizationDataRow("map_A4", "keeper", "journey", "Please try to catch some of them. Please try to pick up some of them. ", "من فضلك، حاول اصطياد البعض منها. من فضلك، حاول التقاطها.", "map_A4", "Please try to catch some of them for us"));
			Rows.Add( new LocalizationDataRow("map1_A1", "keeper", "journey", "You did a great job! We need to investigate this are more. ", "كان ادائك رائعاً! علينا متابعة استكشاف هذه المنطقة", "map1_A1", "You did a great job! We need to continue to explore this area."));
			Rows.Add( new LocalizationDataRow("map1_A2", "keeper", "journey", "I am sure you will find more exciting creatures here. ", "انا واثق  من انك ستجد مخلوقات اكثر غرابة و مثيرة للاعجاب", "map1_A2", "I’m sure we’ll find some more interesting specimens around here"));
			Rows.Add( new LocalizationDataRow("assessment_intro_A1", "keeper", "journey", "You did an awesome job during the previous missions. ", "ادائك كان عظيماً خلال المهمات السابقة", "assessment_intro_A1", "Kid, You did a fantastic job during the last missions."));
			Rows.Add( new LocalizationDataRow("assessment_intro_A2", "keeper", "journey", "We have to stop playing for now. I need to evaluate you. ", "علينا ايقاف اللعب الان. سوف نقوم بتقييم جدي", "assessment_intro_A2", "But we are not playing anymore… I must evaluate you."));
			Rows.Add( new LocalizationDataRow("assessment_intro_A3", "keeper", "journey", "Let's see how good you are in recognizing the living letters in this area. ", "لنرى كم انت ضليع في معرفة الحروف الحية في هذه المنطقة", "assessment_intro_A3", "Do you really know the species living in this region? We’ll see."));
			Rows.Add( new LocalizationDataRow("assessment_intro_A4", "keeper", "journey", "Get ready!", "كن جاهزاً او تحضَر", "assessment_intro_A4", "Be ready"));
			Rows.Add( new LocalizationDataRow("mood_how_are_you_today", "female", "mood", "How are you doing today?", "كيف حالك اليوم؟", "mood_how_are_you_today", "How are you today?"));
			Rows.Add( new LocalizationDataRow("mood_how_do_you_feel", "female", "mood", "How do you feel now?", " كيف تشعر الآن؟", "mood_how_do_you_feel", "How do you feel now?"));
			Rows.Add( new LocalizationDataRow("end_game_A1", "keeper", "playsession", "Thank you very much for your help today.", "شكراً جزيلاً على مساعدتك اليوم", "end_game_A1", "Thank you so much for your help today!"));
			Rows.Add( new LocalizationDataRow("end_game_A2", "keeper", "playsession", "I hope you come soon to explore new areas. ", "امل ان تعود قريباً لاستكشاف مناطق جديدة معنا", "end_game_A2", "I hope you’ll come back soon to explore new regions with us."));
			Rows.Add( new LocalizationDataRow("end_game_A3", "keeper", "playsession", "Goodbye! ", "الى اللقاء!", "end_game_A3", "Bye!"));
			Rows.Add( new LocalizationDataRow("game_result_great_B", "keeper", "results", "Excellent! ", "جيد جداً", "game_result_great_B", "Very well"));
			Rows.Add( new LocalizationDataRow("game_result_good_B", "keeper", "results", "Good, although you can still do better. ", "جيد، انا واثق من قدرتك على تحسين النتيجة", "game_result_good_B", "Good, but you can improve"));
			Rows.Add( new LocalizationDataRow("game_result_retry", "keeper", "results", "No worries, you can re-try next time.", "لا بأس، يمكنك المحاولة لاحقاً", "game_result_retry", "Ahh.. i think you retry next time."));
			Rows.Add( new LocalizationDataRow("game_result_fair", "keeper", "results", "Good, you can get more next time. ", "جيد، سنحصل على المزيد في المرة المقبلة", "game_result_fair", "Good work, we’ll get more next time"));
			Rows.Add( new LocalizationDataRow("game_result_good", "keeper", "results", "Impressive performance!", " نتيجة رائعة!", "game_result_good", "That’s very well done"));
			Rows.Add( new LocalizationDataRow("game_result_great", "keeper", "results", "Wonderful job!", "عمل ممتاز!", "game_result_great", "You did a perfect job!"));
			Rows.Add( new LocalizationDataRow("game_rewards_intro1", "keeper", "results", "End of this mission. Look here! ", "انها نهاية المرحلة. انظر الى هذا!", "game_rewards_intro1", "It’s the end of this mission. And look at that!"));
			Rows.Add( new LocalizationDataRow("game_rewards_intro2", "keeper", "results", "You won some upgrades for Antura", "لقد ربحت شيئاً رائعاً، انها تخصيصات لتعديل شكل عنتورة", "game_rewards_intro2", "It’s seems you win something to customize Antura"));
			Rows.Add( new LocalizationDataRow("game_reward_A1", "keeper", "results", "Congratulations! ", "مبروك", "game_reward_A1", "Congratulation."));
			Rows.Add( new LocalizationDataRow("game_reward_A2", "keeper", "results", "I am sure Antura will love these. ", "انا واثق ان عنتورة سيحب هذه الاضافات", "game_reward_A2", "I’m sure Antura will love it"));
			Rows.Add( new LocalizationDataRow("end_learningblock_A1", "keeper", "results", "You did huge efforts in collecting the living letters. ", "قمت بمجهود عظيم خلال مساهمتك بالتقاط الحروف الحية", "end_learningblock_A1", "You did a lot of efforts to contribute to the Living Letters collection"));
			Rows.Add( new LocalizationDataRow("end_learningblock_A2", "keeper", "results", "I have a special gift for you!", "لدي هدية مميزة لك!", "end_learningblock_A2", "I have a little gift for you"));
			Rows.Add( new LocalizationDataRow("ui_living_letters", "kids or female", "ui", "The living letters. ", "الحروف الحية", "ui_living_letters", "Living Letters"));
			Rows.Add( new LocalizationDataRow("ui_game_title", "kids or female", "ui", "Antura and the letters. ", "عنتورة و الحروف", "ui_game_title", "Antura and the letters!"));
			Rows.Add( new LocalizationDataRow("wheel_A1", "keeper", "wheel", "My dog Antura will help you find the letters. ", "كلبي عنتورة سيساعدك في العثور على الحروف ", "wheel_A1", "My dog Antura will help you to find the letters."));
			Rows.Add( new LocalizationDataRow("wheel_A2", "keeper", "wheel", "He is not focused on his job. ", "لكن التركيز ليس من افضل سماته خلال العمل", "wheel_A2", "but he’s not very focused on his job."));
			Rows.Add( new LocalizationDataRow("wheel_A3", "keeper", "wheel", "Fine! ", "حسناً!", "wheel_A3", "Well!"));
			Rows.Add( new LocalizationDataRow("wheel_A4", "keeper", "wheel", "This is the wheel game which is used to attract the living letters. They love it!", "هذه لعبة الدولاب، نستعملها لجذب انتباه الحروف. انها تعشششق اللعب", "wheel_A4", "This is the wheel game, we use it to attract the living letters. They loooove to play."));
			Rows.Add( new LocalizationDataRow("wheel_A5", "keeper", "wheel", "Come on, let's spin the wheel! ", "هيا بنا، ادر الدولاب!", "wheel_A5", "Come on turn the wheel!"));
			Rows.Add( new LocalizationDataRow("wheel_game_fastcrowd", "keeper", "wheel", "Aha! It is the fast crowd game! Let's see what we can catch today. ", "اااه! لعبة جماهير الحروف! لنرى ماذا سنصطاد اليوم؟", "wheel_game_fastcrowd", "Ah ah! It’s the game Fast crowd! Let see what we can catch with it"));
			Rows.Add( new LocalizationDataRow("wheel_turn", "keeper", "wheel", "Spin the wheel. ", "ادر الدولاب", "wheel_turn", "turn the wheel"));
			Rows.Add( new LocalizationDataRow("wheel_game_balloons_intro", "keeper", "wheel", "It is time to pick up more living letters. ", "حان الوقت لالتقاط المزيد من الحروف!", "wheel_game_balloons_intro", "Time to catch some more living letters!"));
			Rows.Add( new LocalizationDataRow("wheel_game_balloons_end", "keeper", "wheel", "The balloon game! You will catch so many letters with the baloon. ", "لعبة البالونات. سوف تصطاد فيها الكثير من الحروف", "wheel_game_balloons_end", "The ballooooon. You're gonna catch many of them with it"));
			Rows.Add( new LocalizationDataRow("wheel_game_fastcrowdA", "keeper", "wheel", "Hmm, interesting. ", "همممم، مشوق.", "wheel_game_fastcrowdA", "Hummmm, interesting."));
			Rows.Add( new LocalizationDataRow("wheel_locked", "keeper", "wheel", "Okay ", "حسناً", "wheel_locked", "Ok"));
			Rows.Add( new LocalizationDataRow("wheel_game_dontwake", "keeper", "wheel", "Where is Antura? He is sleeping among the living letter creatures. ", "اين هو عنتورة؟ اهه كلا، انه يأخذ قيلولة وسط منطقة الحروف الحية", "wheel_game_dontwake", "Where is Antura? Oh no, he felt asleep among the Living Words specimens…"));
			Rows.Add( new LocalizationDataRow("game_dontwake_fail_antura", "keeper", "game_dontwake", "Do not touch Antura!", "لا تلمس الكلب!", "", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_fail_alarms", "keeper", "game_dontwake", "Do not touch the alarms!", "لا تلمس أجهزة الإنذار!", "", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_fail_toofast", "keeper", "game_dontwake", "Don't go too fast!", "لا تذهب بسرعة كبيرة!", "", ""));
			Rows.Add( new LocalizationDataRow("game_dontwake_fail_fall", "keeper", "game_dontwake", "Don't fall!", "لا تقع!", "", ""));
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
