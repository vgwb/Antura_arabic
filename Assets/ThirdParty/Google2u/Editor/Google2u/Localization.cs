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
	public class LocalizationRow : IGoogle2uRow
	{
		public string _en;
		public string _fr;
		public string _it;
		public string _de;
		public string _es;
		public string _ar;
		public string _zh_cn;
		public string _ja;
		public string _ko;
		public string _vi;
		public string _ru;
		public string _nl;
		public LocalizationRow(string __ID, string __en, string __fr, string __it, string __de, string __es, string __ar, string __zh_cn, string __ja, string __ko, string __vi, string __ru, string __nl) 
		{
			_en = __en.Trim();
			_fr = __fr.Trim();
			_it = __it.Trim();
			_de = __de.Trim();
			_es = __es.Trim();
			_ar = __ar.Trim();
			_zh_cn = __zh_cn.Trim();
			_ja = __ja.Trim();
			_ko = __ko.Trim();
			_vi = __vi.Trim();
			_ru = __ru.Trim();
			_nl = __nl.Trim();
		}

		public int Length { get { return 12; } }

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
					ret = _en.ToString();
					break;
				case 1:
					ret = _fr.ToString();
					break;
				case 2:
					ret = _it.ToString();
					break;
				case 3:
					ret = _de.ToString();
					break;
				case 4:
					ret = _es.ToString();
					break;
				case 5:
					ret = _ar.ToString();
					break;
				case 6:
					ret = _zh_cn.ToString();
					break;
				case 7:
					ret = _ja.ToString();
					break;
				case 8:
					ret = _ko.ToString();
					break;
				case 9:
					ret = _vi.ToString();
					break;
				case 10:
					ret = _ru.ToString();
					break;
				case 11:
					ret = _nl.ToString();
					break;
			}

			return ret;
		}

		public string GetStringData( string colID )
		{
			var ret = System.String.Empty;
			switch( colID )
			{
				case "en":
					ret = _en.ToString();
					break;
				case "fr":
					ret = _fr.ToString();
					break;
				case "it":
					ret = _it.ToString();
					break;
				case "de":
					ret = _de.ToString();
					break;
				case "es":
					ret = _es.ToString();
					break;
				case "ar":
					ret = _ar.ToString();
					break;
				case "zh-cn":
					ret = _zh_cn.ToString();
					break;
				case "ja":
					ret = _ja.ToString();
					break;
				case "ko":
					ret = _ko.ToString();
					break;
				case "vi":
					ret = _vi.ToString();
					break;
				case "ru":
					ret = _ru.ToString();
					break;
				case "nl":
					ret = _nl.ToString();
					break;
			}

			return ret;
		}
		public override string ToString()
		{
			string ret = System.String.Empty;
			ret += "{" + "en" + " : " + _en.ToString() + "} ";
			ret += "{" + "fr" + " : " + _fr.ToString() + "} ";
			ret += "{" + "it" + " : " + _it.ToString() + "} ";
			ret += "{" + "de" + " : " + _de.ToString() + "} ";
			ret += "{" + "es" + " : " + _es.ToString() + "} ";
			ret += "{" + "ar" + " : " + _ar.ToString() + "} ";
			ret += "{" + "zh-cn" + " : " + _zh_cn.ToString() + "} ";
			ret += "{" + "ja" + " : " + _ja.ToString() + "} ";
			ret += "{" + "ko" + " : " + _ko.ToString() + "} ";
			ret += "{" + "vi" + " : " + _vi.ToString() + "} ";
			ret += "{" + "ru" + " : " + _ru.ToString() + "} ";
			ret += "{" + "nl" + " : " + _nl.ToString() + "} ";
			return ret;
		}
	}
	public sealed class Localization : IGoogle2uDB
	{
		public enum rowIds {
			ID_ERROR_EMPTY_URL, ID_ERROR_INVALID_URL, ID_ERROR_INVALID_RESOURCES_DIR, ID_ERROR_INVALID_EDITOR_DIR, ID_ERROR_INVALID_DIR, ID_ERROR_INVALID_UPLOAD_DIR, ID_ERROR_BUILD_TARGET, ID_ERROR_INVALID_WORKSHEET, ID_MESSAGE_QUERYING_WORKBOOKS, ID_MESSAGE_QUERYING_WORKSHEETS, ID_MESSAGE_QUERYING_CELLS, ID_MESSAGE_REMOVE_WORKBOOK, ID_MESSAGE_NO_WORKSHEETS, ID_MESSAGE_EMPTY_WORKSHEET, ID_MESSAGE_LOGGED_IN_AS, ID_MESSAGE_PROCESSING_LOGIN, ID_MESSAGE_RETRIEVING_WORKBOOKS, ID_MESSAGE_UPLOADING_WORKBOOK, ID_MESSAGE_PLAY_MODE
			, ID_LABEL_ACTIVE_WORKSHEET, ID_LABEL_EXPORT_AS, ID_LABEL_OPEN_IN_GOOGLE, ID_LABEL_REFRESH_WORKBOOK, ID_LABEL_EDIT_WORKBOOK, ID_LABEL_VIEW_WORKBOOK, ID_LABEL_REMOVE_WORKBOOK, ID_LABEL_RELOAD_WORKBOOKS, ID_LABEL_DELETE, ID_LABEL_CANCEL, ID_LABEL_EXPORT, ID_LABEL_LOGIN, ID_LABEL_LOGOUT, ID_LABEL_CREDENTIALS, ID_LABEL_SAVE_CREDENTIALS, ID_LABEL_AUTO_LOGIN, ID_LABEL_USERNAME, ID_LABEL_PASSWORD, ID_LABEL_SETTINGS, ID_LABEL_LANGUAGE
			, ID_LABEL_EDITOR_LANGUAGE, ID_LABEL_EXPORTERS, ID_LABEL_GENERATE_PATHS, ID_LABEL_ENABLE, ID_LABEL_GAME_OBJECT_DATABASE, ID_LABEL_OBJECT_DATABASE, ID_LABEL_STATIC_DATABASE, ID_LABEL_RESOURCES_DIR, ID_LABEL_EDITOR_DIR, ID_LABEL_EXPORT_DIR, ID_LABEL_CHOOSE_FOLDER, ID_LABEL_WORKBOOKS, ID_LABEL_MANUAL_WORKBOOKS, ID_LABEL_ACCOUNT_WORKBOOKS, ID_LABEL_ADD_WORKBOOK, ID_LABEL_UPLOAD_WORKBOOK, ID_LABEL_COMPLETE, ID_LABEL_CHOOSE_FILE, ID_LABEL_SELECT_WORKBOOK_PATH, ID_LABEL_HELP
			, ID_LABEL_CONTACT, ID_LABEL_BROWSE_LITTERATUS, ID_LABEL_BROWSE_UNITY, ID_LABEL_CREATED_WITH_UNITY, ID_LABEL_COPYRIGHT_UNITY, ID_LABEL_DOCUMENTATION, ID_LABEL_UPDATE, ID_LABEL_SYNC, ID_LABEL_EXPORT_OPTIONS, ID_LABEL_WHITESPACE, ID_LABEL_TRIM_STRINGS, ID_LABEL_TRIM_STRING_ARRAYS, ID_LABEL_ARRAY_DELIMITERS, ID_LABEL_NON_STRING, ID_LABEL_STRINGS, ID_LABEL_COMPLEX_TYPES, ID_LABEL_COMPLEX_ARRAYS, ID_LABEL_CREATION_OPTIONS, ID_LABEL_GENERATE_PLAYMAKER, ID_LABEL_PERSIST_SCENE_LOADING
			, ID_LABEL_JSON_FORMATTING, ID_LABEL_JSON_EXPORT_CLASS, ID_LABEL_JSON_EXPORT_TYPE, ID_LABEL_ESCAPE_UNICODE, ID_LABEL_CONVERT_CELL_ARRAYS, ID_LABEL_VALIDATE_WORKSHEET, ID_LABEL_VALIDATE_WORKBOOK, ID_LABEL_CSV_FORMATTING, ID_LABEL_NGUI_FORMATTING, ID_LABEL_ESCAPE_STRINGS, ID_LABEL_JSON_OBJECT_PREVIEW, ID_LABEL_JSON_CLASS_PREVIEW, ID_LABEL_GENERATE_PREVIEW, ID_LABEL_GENERATE_CLASS, ID_LABEL_CSV_PREVIEW, ID_LABEL_NGUI_PREVIEW, ID_LABEL_XML_PREVIEW, ID_LABEL_XML_FORMATTING, ID_LABEL_SHOWONSTARTUP, ID_LABEL_CULL_COLUMNS
			, ID_LABEL_CULL_ROWS, ID_LABEL_ESCAPE_LINE_BREAKS, ID_LABEL_LEGACY_OPTIONS, ID_LABEL_LOWERCASE_HEADER, ID_LABEL_TYPEROWBOX_HEADER, ID_LABEL_TYPEROWBOX_MESSAGE
		};
		public string [] rowNames = {
			"ID_ERROR_EMPTY_URL", "ID_ERROR_INVALID_URL", "ID_ERROR_INVALID_RESOURCES_DIR", "ID_ERROR_INVALID_EDITOR_DIR", "ID_ERROR_INVALID_DIR", "ID_ERROR_INVALID_UPLOAD_DIR", "ID_ERROR_BUILD_TARGET", "ID_ERROR_INVALID_WORKSHEET", "ID_MESSAGE_QUERYING_WORKBOOKS", "ID_MESSAGE_QUERYING_WORKSHEETS", "ID_MESSAGE_QUERYING_CELLS", "ID_MESSAGE_REMOVE_WORKBOOK", "ID_MESSAGE_NO_WORKSHEETS", "ID_MESSAGE_EMPTY_WORKSHEET", "ID_MESSAGE_LOGGED_IN_AS", "ID_MESSAGE_PROCESSING_LOGIN", "ID_MESSAGE_RETRIEVING_WORKBOOKS", "ID_MESSAGE_UPLOADING_WORKBOOK", "ID_MESSAGE_PLAY_MODE"
			, "ID_LABEL_ACTIVE_WORKSHEET", "ID_LABEL_EXPORT_AS", "ID_LABEL_OPEN_IN_GOOGLE", "ID_LABEL_REFRESH_WORKBOOK", "ID_LABEL_EDIT_WORKBOOK", "ID_LABEL_VIEW_WORKBOOK", "ID_LABEL_REMOVE_WORKBOOK", "ID_LABEL_RELOAD_WORKBOOKS", "ID_LABEL_DELETE", "ID_LABEL_CANCEL", "ID_LABEL_EXPORT", "ID_LABEL_LOGIN", "ID_LABEL_LOGOUT", "ID_LABEL_CREDENTIALS", "ID_LABEL_SAVE_CREDENTIALS", "ID_LABEL_AUTO_LOGIN", "ID_LABEL_USERNAME", "ID_LABEL_PASSWORD", "ID_LABEL_SETTINGS", "ID_LABEL_LANGUAGE"
			, "ID_LABEL_EDITOR_LANGUAGE", "ID_LABEL_EXPORTERS", "ID_LABEL_GENERATE_PATHS", "ID_LABEL_ENABLE", "ID_LABEL_GAME_OBJECT_DATABASE", "ID_LABEL_OBJECT_DATABASE", "ID_LABEL_STATIC_DATABASE", "ID_LABEL_RESOURCES_DIR", "ID_LABEL_EDITOR_DIR", "ID_LABEL_EXPORT_DIR", "ID_LABEL_CHOOSE_FOLDER", "ID_LABEL_WORKBOOKS", "ID_LABEL_MANUAL_WORKBOOKS", "ID_LABEL_ACCOUNT_WORKBOOKS", "ID_LABEL_ADD_WORKBOOK", "ID_LABEL_UPLOAD_WORKBOOK", "ID_LABEL_COMPLETE", "ID_LABEL_CHOOSE_FILE", "ID_LABEL_SELECT_WORKBOOK_PATH", "ID_LABEL_HELP"
			, "ID_LABEL_CONTACT", "ID_LABEL_BROWSE_LITTERATUS", "ID_LABEL_BROWSE_UNITY", "ID_LABEL_CREATED_WITH_UNITY", "ID_LABEL_COPYRIGHT_UNITY", "ID_LABEL_DOCUMENTATION", "ID_LABEL_UPDATE", "ID_LABEL_SYNC", "ID_LABEL_EXPORT_OPTIONS", "ID_LABEL_WHITESPACE", "ID_LABEL_TRIM_STRINGS", "ID_LABEL_TRIM_STRING_ARRAYS", "ID_LABEL_ARRAY_DELIMITERS", "ID_LABEL_NON_STRING", "ID_LABEL_STRINGS", "ID_LABEL_COMPLEX_TYPES", "ID_LABEL_COMPLEX_ARRAYS", "ID_LABEL_CREATION_OPTIONS", "ID_LABEL_GENERATE_PLAYMAKER", "ID_LABEL_PERSIST_SCENE_LOADING"
			, "ID_LABEL_JSON_FORMATTING", "ID_LABEL_JSON_EXPORT_CLASS", "ID_LABEL_JSON_EXPORT_TYPE", "ID_LABEL_ESCAPE_UNICODE", "ID_LABEL_CONVERT_CELL_ARRAYS", "ID_LABEL_VALIDATE_WORKSHEET", "ID_LABEL_VALIDATE_WORKBOOK", "ID_LABEL_CSV_FORMATTING", "ID_LABEL_NGUI_FORMATTING", "ID_LABEL_ESCAPE_STRINGS", "ID_LABEL_JSON_OBJECT_PREVIEW", "ID_LABEL_JSON_CLASS_PREVIEW", "ID_LABEL_GENERATE_PREVIEW", "ID_LABEL_GENERATE_CLASS", "ID_LABEL_CSV_PREVIEW", "ID_LABEL_NGUI_PREVIEW", "ID_LABEL_XML_PREVIEW", "ID_LABEL_XML_FORMATTING", "ID_LABEL_SHOWONSTARTUP", "ID_LABEL_CULL_COLUMNS"
			, "ID_LABEL_CULL_ROWS", "ID_LABEL_ESCAPE_LINE_BREAKS", "ID_LABEL_LEGACY_OPTIONS", "ID_LABEL_LOWERCASE_HEADER", "ID_LABEL_TYPEROWBOX_HEADER", "ID_LABEL_TYPEROWBOX_MESSAGE"
		};
		public System.Collections.Generic.List<LocalizationRow> Rows = new System.Collections.Generic.List<LocalizationRow>();

		public static Localization Instance
		{
			get { return NestedLocalization.instance; }
		}

		private class NestedLocalization
		{
			static NestedLocalization() { }
			internal static readonly Localization instance = new Localization();
		}

		private Localization()
		{
			Rows.Add( new LocalizationRow("ID_ERROR_EMPTY_URL", "The URL is empty", "L'URL est vide", "L'URL è vuoto", "Die URL ist leer", "La URL está vacía", "وURL فارغ", "网址是空的", "URLは空です", "URL은 비어 있습니다", "Các URL có sản phẩm nào", "Ссылка пуст", "De URL is leeg"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_URL", "The URL is invalid", "L'URL est invalide", "L'URL non è valido", "Die URL ist ungültig", "La URL no es válida", "وURL غير صالح", "网址无效", "URLが無効です", "URL이 유효하지 않습니다", "Các URL không hợp lệ", "Ссылка является недействительным", "De URL is ongeldig"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_RESOURCES_DIR", "You must choose an resources directory within this project", "Vous devez choisir un répertoire des ressources au sein de ce projet", "È necessario scegliere una directory di risorse in questo progetto", "Sie müssen ein Verzeichnis Ressourcen im Rahmen dieses Projektes wählen", "Usted debe elegir un directorio de recursos dentro de este proyecto", "يجب عليك اختيار دليل الموارد ضمن هذا المشروع", "你必须选择这个项目中的资源目录", "あなたは、このプロジェクト内のリソース·ディレクトリーを選択する必要があります", "이 프로젝트에서 자원 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục nguồn lực trong dự án này", "Вы должны выбрать каталог ресурсов в рамках этого проекта", "U moet een resources directory binnen dit project kiezen"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_EDITOR_DIR", "You must choose an editor directory within this project", "Vous devez choisir un répertoire de l'éditeur au sein de ce projet", "È necessario scegliere una directory editore nell'ambito di questo progetto", "Sie müssen einen Editor Verzeichnis innerhalb dieses Projektes wählen", "Usted debe elegir un directorio editor dentro de este proyecto", "يجب عليك اختيار دليل المحرر ضمن هذا المشروع", "你必须选择这个项目中的一个编辑器目录", "あなたは、このプロジェクト内エディタディレクトリを選択する必要があります", "이 프로젝트에서 에디터 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục trong trình soạn thảo dự án này", "Вы должны выбрать директорию редактор в рамках этого проекта", "U moet een map editor binnen dit project kiezen"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_DIR", "You must choose a directory within this project", "Vous devez choisir un répertoire dans ce projet", "È necessario scegliere una directory all'interno di questo progetto", "Sie müssen ein Verzeichnis innerhalb dieses Projektes wählen", "Usted debe elegir un directorio dentro de este proyecto", "يجب عليك اختيار الدليل ضمن هذا المشروع", "你必须选择这个项目中的目录", "あなたは、このプロジェクト内のディレクトリを選択する必要があります", "이 프로젝트 내에서 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục trong dự án này", "Вы должны выбрать папку, в этом проекте", "U moet een map binnen dit project kiezen"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_UPLOAD_DIR", "Workbook Upload Path Invalid", "Classeur chargement Chemin incorrect", "Workbook Carica Percorso non valido", "Workbook hochladen Pfad ungültig", "Cuaderno Upload Path no válido", "المصنف رفع مسار غير صالح", "工作簿上传路径无效", "ワークブックアップロードパスが無効です", "통합 문서 업로드 경로가 잘못되었습니다", "Workbook Tải lên Đường dẫn không hợp lệ", "Рабочая тетрадь Загрузить Путь Неверный", "Werkboek uploaden Path Ongeldige"));
			Rows.Add( new LocalizationRow("ID_ERROR_BUILD_TARGET", "Google2u is unable to communicate with Google using this build target. Please switch to either the Standalone or Mobile targets.", "Google2u est incapable de communiquer avec Google utilisant cette cible de construction. S'il vous plaît passer soit à l'autonome ou cibles mobiles.", "Google2u non riesce a comunicare con Google con questo target build. Si prega di passare a uno il Standalone o bersagli mobili.", "Google2u ist nicht mit Google mit dieser Build-Ziel zu kommunizieren. Bitte entweder an den Standalone oder bewegliche Ziele zu wechseln.", "Google2u es incapaz de comunicarse con Google que utiliza este tipo de generación. Por favor, cambiar a cualquiera de la Independiente o blancos móviles.", "Google2u غير قادر على التواصل مع جوجل باستخدام هذا الهدف بناء. يرجى التبديل إلى إما بذاتها أو أهداف متحركة.", "Google2u是无法与谷歌使用这个版本的目标进行通信。请切换到无论是独立或移动目标。", "Google2uは、このビルドターゲットを使用してGoogleと通信することができません。スタンドアロンまたはモバイルのターゲットのいずれかに切り替えてください。", "Google2u 구글이 빌드 타겟을 이용하여 통신 할 수 없습니다. 독립 또는 모바일 목표 중 하나로 전환하십시오.", "Google2u là không thể giao tiếp với Google bằng cách sử dụng xây dựng mục tiêu này. Hãy chuyển đến hoặc là độc lập hoặc mục tiêu di động.", "Google2u не в состоянии общаться с Google, используя этот сборки цель. Пожалуйста, переключиться в режим автономного или подвижных целей.", "Google2u niet in staat is om te communiceren met Google met behulp van deze build doel. Schakel om ofwel de Standalone of Mobile targets."));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_WORKSHEET", "Invalid Worksheet", "Feuille non valide", "Foglio di lavoro non valido", "Ungültige Arbeitsblatt", "Hoja de trabajo no válido", "ورقة عمل غير صالح", "工作表无效", "無効なワークシート", "잘못된 워크 시트", "Worksheet không hợp lệ", "Неверный лист", "Ongeldige werkblad"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_WORKBOOKS", "Querying Workbooks", "Interrogation classeurs", "Interrogazione cartelle di lavoro", "Abfragen von Arbeitsmappen", "Consulta Libros", "الاستعلام عن المصنفات", "查询工作簿", "問合せワークブック", "쿼리 통합 문서", "Truy vấn Workbooks", "Запросы Рабочие тетради", "Bevraging werkmappen"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_WORKSHEETS", "Querying Worksheets", "Interrogation Feuilles", "Interrogazione Fogli di lavoro", "Abfragen von Arbeitsblättern", "Consulta Hojas de trabajo", "الاستعلام عن أوراق العمل", "查询工作表", "問合せワークシート", "쿼리 워크 시트", "Truy vấn Worksheets", "Запросы листы", "Bevraging Werkbladen"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_CELLS", "Querying Cells", "Interrogation cellules", "Interrogazione Cells", "Abfragen Cells", "Consulta de células", "الاستعلام عن الخلايا", "查询细胞", "細胞への問合せ", "세포 쿼리", "Truy vấn Cells", "Запрос клеток", "Opvragen Cellen"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_REMOVE_WORKBOOK", "Remove the Manual Workbook? This cannot be undone.", "Retirez le classeur Manuel? Ça ne peut pas être annulé.", "Rimuovere la cartella di lavoro manuale? Questo non può essere annullata.", "Entfernen Sie die Handarbeitsbuch? Das kann nicht rückgängig gemacht werden.", "Retire el Libro de Trabajo Manual? Esto no se puede deshacer.", "إزالة مصنف دليل؟ هذا لا يمكن التراجع عنها.", "取下手动工作簿？这是不能取消的。", "マニュアルブックを削除しますか？後戻りはできません。", "수동 통합 문서를 삭제 하시겠습니까? 이 취소 할 수 없습니다.", "Tháo Workbook tay? Điều này không thể được hoàn tác.", "Удалить ручной Workbook? Это не может быть отменено.", "Verwijder de handmatige Werkboek? Dit kan niet ongedaan worden."));
			Rows.Add( new LocalizationRow("ID_MESSAGE_NO_WORKSHEETS", "No Worksheets Found", "Aucun Feuilles trouvés", "Nessun fogli di lavoro trovato", "Keine Arbeitsblätter gefunden", "No hay hojas de trabajo encontrados", "لا أوراق العمل التي تم العثور عليها", "没有找到工作表", "いいえワークシートが見つかりません", "어떤 워크 시트를 찾을 수 없습니다", "Không Worksheets Found", "Нет Рабочие не найдены", "Geen Werkbladen Gevonden"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_EMPTY_WORKSHEET", "Empty Worksheet", "Feuille vide", "Foglio di lavoro vuoto", "Leeres Arbeitsblatt", "Hoja de trabajo vacía", "ورقة عمل فارغة", "空工作表", "空のワークシート", "빈 워크 시트", "Rỗng Worksheet", "Пустой лист", "Leeg werkblad"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_LOGGED_IN_AS", "Logged In", "Connecté", "Connesso", "Eingeloggt", "Conectado", "تسجيل الدخول", "登录", "ログインしているユーザー", "로그인", "Đăng Nhập", "Записан В", "Ingelogd"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_PROCESSING_LOGIN", "Processing Login, Please Wait", "Traitement Connexion, S'il vous plaît Attendez", "Elaborazione accesso, Attendere prego", "Verarbeitungs Anmelden, bitte warten", "Procesamiento de inicio de sesión, Please Wait", "تجهيز تسجيل الدخول، يرجى الانتظار", "处理登录，请稍候", "ログインを処理して、お待ちください", "처리 로그인, 잠시 기다려주십시오", "Chế biến Login, Please Wait", "Обработка Войти, пожалуйста, подождите", "Processing Login, Please Wait"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_RETRIEVING_WORKBOOKS", "Retrieving Workbooks, Please Wait", "Récupération des classeurs, S'il vous plaît Attendez", "Recupero cartelle di lavoro, Attendere prego", "Abrufen von Arbeitsmappen, bitte warten", "Recuperando Libros, espera por favor", "استرجاع المصنفات، من فضلك انتظر", "检索工作簿，请稍候", "ワークブックの取得、お待ちください", "통합 문서를 가져 오는 중, 잠시 기다려주십시오", "Lấy Workbooks, Please Wait", "Получение рабочих книг, пожалуйста, подождите", "Ophalen werkboeken, Please Wait"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_UPLOADING_WORKBOOK", "Uploading Workbook", "L'ajout Workbook", "Caricamento Workbook", "Hochladen Workbook", "Cargar Workbook", "تحميل مصنف", "上传工作簿", "アップロードワークブック", "업로드 통합 문서", "Uploading Workbook", "Загрузка пособие", "Uploaden Werkboek"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_PLAY_MODE", "Google2u is an Editor-Only application. Functionality is unavailable in Play Mode.", "Google2u est une application Editor seule. La fonctionnalité est indisponible en mode Play.", "Google2u è un'applicazione Editor-Only. La funzionalità non è disponibile in modalità Play.", "Google2u ist ein Editor-Only-Anwendung. Die Funktionalität ist im Play-Modus nicht verfügbar.", "Google2u es una aplicación Editor Only. Funcionalidad no está disponible en el modo de reproducción.", "Google2u هو تطبيق محرر فقط. وظائف غير متوفر في طريقة اللعب.", "Google2u是一个编辑器，只有应用程序。功能在播放模式下无法使用。", "Google2uは、エディタ専用のアプリケーションです。機能は、プレイモードでは使用できません。", "Google2u는 편집기 전용 응용 프로그램입니다. 기능은 재생 모드에서 사용할 수 없습니다.", "Google2u là một ứng dụng biên tập-Only. Chức năng là không có sẵn trong Play Mode.", "Google2u это приложение редактор только. Функциональность недоступен в режиме воспроизведения.", "Google2u is een Editor-Only applicatie. Functionaliteit is niet beschikbaar in de Play Mode."));
			Rows.Add( new LocalizationRow("ID_LABEL_ACTIVE_WORKSHEET", "Active Worksheet", "Feuille active", "Foglio di lavoro attivo", "Aktive Arbeitsblatt", "Hoja Activo", "ورقة عمل نشط", "活动工作表", "アクティブなワークシート", "현재 워크 시트", "Hoạt động Worksheet", "Активный лист", "Actieve werkblad"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_AS", "Export as", "Exporter comme", "Esportazione come", "Export als", "Exportar como", "التصدير،", "作为出口", "としてエクスポート", "로 내보내기", "Export as", "Экспорт в", "Exporteren als"));
			Rows.Add( new LocalizationRow("ID_LABEL_OPEN_IN_GOOGLE", "Open Workbook in Google", "Ouvrir un classeur dans Google", "Apri cartella di lavoro in Google", "Arbeitsmappe öffnen in Google", "Abrir libro en Google", "فتح المصنف في جوجل", "在谷歌打开工作簿", "グーグルで開くワークブック", "구글에서 열기 통합 문서", "Mở Workbook trong Google", "Открыть книгу в Google", "Open Werkboek in Google"));
			Rows.Add( new LocalizationRow("ID_LABEL_REFRESH_WORKBOOK", "Refresh Workbook", "Actualiser classeur", "Aggiorna cartella di lavoro", "Refresh-Arbeitsmappe", "Actualizar Libro de Trabajo", "تحديث مصنف", "刷新工作簿", "リフレッシュワークブック", "새로 고침 통합 문서", "Refresh Workbook", "Обновить пособие", "Vernieuwen Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDIT_WORKBOOK", "Edit Workbook", "Modifier Workbook", "Modifica Workbook", "Bearbeiten-Arbeitsmappe", "Editar Libro de Trabajo", "تحرير مصنف", "编辑工作簿", "編集ワークブック", "편집 워크 북", "Sửa Workbook", "Редактировать пособие", "Bewerken Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_VIEW_WORKBOOK", "View Workbook", "Voir Workbook", "Vista Workbook", "Ansicht Workbook", "Ver Libro de Trabajo", "عرض مصنف", "查看工作簿", "見るワークブック", "통합 문서보기", "Xem Workbook", "Посмотреть пособие", "Bekijk Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_REMOVE_WORKBOOK", "Remove Workbook", "Retirer Workbook", "Rimuovere Workbook", "Entfernen Sie Workbook", "Retire Workbook", "إزالة مصنف", "删除工作簿", "ワークブックを削除", "통합 문서를 제거", "Di Workbook", "Удалить Workbook", "Verwijder Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_RELOAD_WORKBOOKS", "Reload Workbooks", "Recharger classeurs", "Ricarica cartelle di lavoro", "Reload-Arbeitsmappen", "Recargar Libros", "تحديث المصنفات", "刷新工作簿", "リロードワークブック", "새로 고침 통합 문서", "Nạp lại Workbooks", "Перезагрузить Рабочие тетради", "Reload werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_DELETE", "Delete", "Effacer", "Cancellare", "Löschen", "Borrar", "حذف", "删除", "削除", "삭제", "Xóa bỏ", "Удалить", "Verwijder"));
			Rows.Add( new LocalizationRow("ID_LABEL_CANCEL", "Cancel", "Annuler", "Cancellare", "Stornieren", "Cancelar", "إلغاء", "取消", "キャンセル", "취소", "Hủy bỏ", "Отменить", "Annuleren"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT", "Export", "Exportation", "Esportazione", "Export", "Exportación", "تصدير", "出口", "エクスポート", "수출", "Xuất khẩu", "Экспорт", "Export"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOGIN", "Log In", "S'identifier", "Login", "Einloggen", "Iniciar Sesión", "تسجيل الدخول", "登录", "ログイン", "로그인", "Đăng Nhập", "Войти", "Log In"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOGOUT", "Log Out", "Se Déconnecter", "Uscire", "Abmelden", "Cerrar Sesión", "خروج", "登出", "ログアウト", "로그 아웃", "Đăng Xuất", "Выйти", "Uitloggen"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREDENTIALS", "Credentials", "Lettres de créance", "Credenziali", "Zeugnisse", "Cartas credenciales", "أوراق اعتماد", "证书", "証明", "신임장", "Credentials", "Полномочия", "Geloofsbrieven"));
			Rows.Add( new LocalizationRow("ID_LABEL_SAVE_CREDENTIALS", "Save Credentials", "Économisez de vérification des pouvoirs", "Salva credenziali", "Sparen Sie Credentials", "Guardar credenciales", "حفظ وثائق التفويض", "保存凭证", "資格情報を保存", "자격 증명을 저장", "Lưu Credentials", "Сохранить верительные грамоты", "Sla Geloofsbrieven"));
			Rows.Add( new LocalizationRow("ID_LABEL_AUTO_LOGIN", "Auto Login", "Connexion Automatique", "Login Automatico", "Automatische Anmeldung", "Ingreso Automático", "دخول السيارات", "自动登录", "自動ログイン", "자동 로그인", "Tự động đăng nhập", "Авто Войти", "Auto Login"));
			Rows.Add( new LocalizationRow("ID_LABEL_USERNAME", "User Name", "Nom d'utilisateur", "Nome utente", "Benutzername", "Nombre de Usuario", "اسم المستخدم", "用户名", "ユーザー名", "사용자 이름", "Tên tài khoản", "Имя пользователя", "Gebruikersnaam"));
			Rows.Add( new LocalizationRow("ID_LABEL_PASSWORD", "Password", "Mot de passe", "Password", "Passwort", "Contraseña", "كلمة السر", "密码", "パスワード", "암호", "Mật khẩu", "Пароль", "Wachtwoord"));
			Rows.Add( new LocalizationRow("ID_LABEL_SETTINGS", "Settings", "Paramètres", "Impostazioni", "Einstellungen", "Ajustes", "الإعدادات", "设置", "[設定]", "설정", "Cài đặt", "Настройки", "Instellingen"));
			Rows.Add( new LocalizationRow("ID_LABEL_LANGUAGE", "Language", "Langue", "Lingua", "Sprache", "Idioma", "لغة", "语言", "言語", "언어", "Ngôn ngữ", "Язык", "Taal"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDITOR_LANGUAGE", "Editor Language", "Éditeur Langue", "Editor lingue", "Editor Sprache", "Editor de idiomas", "محرر اللغة", "编辑语言", "エディタ言語", "에디터 언어", "Biên tập viên Ngôn ngữ", "Редактор Язык", "Editor Taal"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORTERS", "Export Paths", "Chemins exportation", "Export Percorsi", "Export Paths", "Exportación Caminos", "تصدير مسارات", "导出路径", "エクスポートパス", "수출 경로", "Xuất Paths", "Экспорт Пути", "Export Paths"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PATHS", "Generate Default Paths", "Générer Chemins par défaut", "Genera percorsi predefiniti", "Generieren Sie Standardpfade", "Generar Rutas predeterminadas", "توليد مسارات افتراضي", "生成默认路径", "デフォルトのパスを生成します", "기본 경로를 생성", "Tạo đường dẫn mặc định", "Создание умолчанию путей", "Genereer Default Paths"));
			Rows.Add( new LocalizationRow("ID_LABEL_ENABLE", "Enable", "Permettre", "Permettere", "Aktivieren", "Habilitar", "تمكين", "启用", "有効にします", "사용", "Cho phép", "Включить", "In staat stellen"));
			Rows.Add( new LocalizationRow("ID_LABEL_GAME_OBJECT_DATABASE", "Game Object Database", "Jeu Object Database", "Gioco Object Database", "Spiel Object Database", "Juego de objetos de base de datos", "لعبة كائن قاعدة بيانات", "游戏对象数据库", "ゲームのオブジェクトデータベース", "게임 객체 데이터베이스", "Game Object Database", "База игр Объект", "Game Object Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_OBJECT_DATABASE", "Object Database", "Base de données de l'objet", "Object Database", "Objektdatenbank", "Base de datos de objetos", "قاعدة بيانات الكائن", "对象数据库", "オブジェクトデータベース", "객체 데이터베이스", "Object Database", "База данных объектов", "Object Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_STATIC_DATABASE", "Static Database", "Base de données statique", "Database Static", "Static Database", "Base de datos estática", "قاعدة بيانات ثابتة", "静态数据库", "静的データベース", "정적 데이터베이스", "Cơ sở dữ liệu tĩnh", "Статический базы данных", "Statische Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_RESOURCES_DIR", "Resources Directory", "Répertoire des ressources", "Risorse Directory", "Ressourcen-Verzeichnis", "Recursos Directorio", "دليل الموارد", "资源目录", "リソースディレクトリ", "자원 디렉토리", "Tài mục", "Каталог ресурсов", "Resources Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDITOR_DIR", "Editor Directory", "Directory Editor", "Directory Editor", "Editor-Verzeichnis", "Directory Editor", "محرر دليل", "编辑目录", "エディタディレクトリ", "에디터 디렉토리", "Biên tập viên mục", "Редактор Каталог", "Editor Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_DIR", "Export Directory", "Exporter l'annuaire", "Directory Export", "Exportverzeichnis", "Directorio de Exportación", "دليل الصادرات", "导出目录", "エクスポートディレクトリ", "수출 디렉토리", "Thư mục Xuất", "Экспорт Каталог", "Export Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_CHOOSE_FOLDER", "Choose a Folder", "Choisissez un dossier", "Scegli una cartella", "Wählen Sie einen Ordner", "Elija una carpeta", "اختيار مجلد", "选择一个文件夹", "フォルダを選択してください", "폴더 선택", "Chọn một thư mục", "Выберите папку", "Kies een map"));
			Rows.Add( new LocalizationRow("ID_LABEL_WORKBOOKS", "Workbooks", "Classeurs", "Cartelle di lavoro", "Arbeitsmappen", "Libros de Trabajo", "المصنفات", "工作簿", "ワークブック", "통합 문서", "Workbooks", "Рабочие тетради", "Werkboeken"));
			Rows.Add( new LocalizationRow("ID_LABEL_MANUAL_WORKBOOKS", "Manual Workbooks", "Classeurs Manuel", "Cartelle di lavoro Manuale", "Manuelle Arbeitsmappen", "Libros Manuales", "المصنفات اليدوية", "手动工作簿", "マニュアルワークブック", "수동 통합 문서", "Workbooks Manual", "Руководство Учебные пособия", "Manual werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_ACCOUNT_WORKBOOKS", "Account Workbooks", "Compte classeurs", "Account cartelle di lavoro", "Konto-Arbeitsmappen", "Cuenta Libros", "حساب المصنفات", "帐户工作簿", "アカウントワークブック", "계정 통합 문서", "Tài khoản Workbooks", "Счет Рабочие тетради", "Account werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_ADD_WORKBOOK", "Add Manual Workbook", "Ajouter classeur Manuel", "Aggiungi cartella di lavoro manuale", "In Handarbeitsbuch", "Añadir Cuaderno Manual", "إضافة دليل مصنف", "添加手动工作簿", "マニュアルブックを追加します。", "수동 통합 문서 추가", "Thêm Workbook Manual", "Добавить Ручная Workbook", "Voeg Manual Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_UPLOAD_WORKBOOK", "Upload Workbook", "Ajouter Workbook", "Carica Workbook", "Upload-Arbeitsmappe", "Subir Workbook", "تحميل مصنف", "上传工作簿", "アップロードワークブック", "업로드 통합 문서", "Tải lên Workbook", "Загрузить пособие", "Uploaden Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLETE", "Complete", "Complet", "Completo", "Komplett", "Completo", "كامل", "完成", "完全な", "완전한", "Hoàn toàn", "Полный", "Compleet"));
			Rows.Add( new LocalizationRow("ID_LABEL_CHOOSE_FILE", "Choose a File to Upload", "Choisissez un fichier à télécharger", "Scegli un file da caricare", "Wählen Sie eine Datei zum hochladen", "Elegir un archivo para subir", "اختيار ملف للتحميل", "选择要上传的文件", "アップロードするファイルを選択してください", "업로드 할 파일을 선택합니다", "Chọn một tập tin để tải lên", "Выберите файл для загрузки", "Kies een bestand te uploaden"));
			Rows.Add( new LocalizationRow("ID_LABEL_SELECT_WORKBOOK_PATH", "Select Workbook Path", "Sélectionnez Chemin Workbook", "Seleziona cartella di lavoro Percorso", "Wählen Sie Workbook-Pfad", "Seleccione Cuaderno Sendero", "تحديد مسار مصنف", "选择工作簿路径", "ブックのパスを選択します", "통합 문서의 경로를 선택", "Chọn Workbook Đường dẫn", "Выберите Рабочего Пути", "Selecteer Werkboek Path"));
			Rows.Add( new LocalizationRow("ID_LABEL_HELP", "Help", "Aidez-Moi", "Aiuto", "Hilfe", "Ayuda", "مساعدة", "救命", "助けて", "도와주세요", "Giúp", "Помогите", "Help"));
			Rows.Add( new LocalizationRow("ID_LABEL_CONTACT", "Contact", "Contact", "Contatto", "Kontakt", "Contacto", "اتصال", "联系", "コンタクト", "접촉", "Tiếp xúc", "Контакт", "Contact"));
			Rows.Add( new LocalizationRow("ID_LABEL_BROWSE_LITTERATUS", "Browse to Litteratus.net", "Accédez à Litteratus.net", "Individuare Litteratus.net", "Navigieren Sie zu Litteratus.net", "Busque Litteratus.net", "استعرض للوصول إلى Litteratus.net", "浏览到Litteratus.net", "Litteratus.netを参照", "Litteratus.net로 이동", "Duyệt đến Litteratus.net", "Найдите Litteratus.net", "Blader naar Litteratus.net"));
			Rows.Add( new LocalizationRow("ID_LABEL_BROWSE_UNITY", "Browse to Unity3d.com", "Accédez à Unity3d.com", "Individuare Unity3d.com", "Navigieren Sie zu Unity3d.com", "Busque Unity3d.com", "استعرض للوصول إلى Unity3d.com", "浏览到Unity3d.com", "Unity3d.comを参照", "Unity3d.com로 이동", "Duyệt đến Unity3d.com", "Найдите Unity3d.com", "Blader naar Unity3d.com"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREATED_WITH_UNITY", "Created with Unity", "Créé avec Unity", "Creato con Unity", "Erstellt mit Unity", "Creado con Unity", "تم إنشاؤها باستخدام الوحدة", "创建与统一", "Unityと作成", "유니티로 만든", "Tạo ra với Unity", "Созданный с единства", "Gemaakt met Unity"));
			Rows.Add( new LocalizationRow("ID_LABEL_COPYRIGHT_UNITY", "Copyright 2014", "Droit d'auteur 2014", "Copyright 2014", "Copyright 2014", "Derechos de Autor 2014", "حقوق الطبع والنشر 2014", "2014年版权所有", "著作権2014", "저작권 2014", "Copyright 2014", "Copyright 2014", "Copyright 2014"));
			Rows.Add( new LocalizationRow("ID_LABEL_DOCUMENTATION", "Documentation", "Documentation", "Documentazione", "Dokumentation", "Documentación", "توثيق", "文件", "ドキュメンテーション", "설명서", "Tài liệu", "Документация", "Documentatie"));
			Rows.Add( new LocalizationRow("ID_LABEL_UPDATE", "Update", "Mise À Jour", "Aggiornare", "Aktualisierung", "Actualización", "التحديث", "更新", "アップデート", "업데이트", "Cập nhật", "Обновление", "Update"));
			Rows.Add( new LocalizationRow("ID_LABEL_SYNC", "Sync with Google", "Synchronisation avec Google", "Sincronizzazione con Google", "Sync mit Google", "Sincronización con Google", "مزامنة مع جوجل", "同步与谷歌", "Googleとの同期", "구글 동기화", "Đồng bộ hóa với Google", "Синхронизация с Google", "Synchroniseren met Google"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_OPTIONS", "Export Options", "Options d'export", "Opzioni di esportazione", "Exportoptionen", "Opciones de exportación", "خيارات التصدير", "导出选项", "エクスポートオプション", "내보내기 옵션", "Tùy chọn Export", "Параметры экспорта", "Export Options"));
			Rows.Add( new LocalizationRow("ID_LABEL_WHITESPACE", "Whitespace", "Les espaces", "Lo spazio bianco", "Whitespace", "El espacio en blanco", "بيضاء", "空白", "空白", "공백", "Khoảng trắng", "Пробелы", "Whitespace"));
			Rows.Add( new LocalizationRow("ID_LABEL_TRIM_STRINGS", "Trim Strings", "Cordes compensateurs", "Strings Trim", "Trim Streicher", "Cuerdas del ajuste", "سلاسل تقليم", "修剪字符串", "文字列のトリム", "트림 문자열", "Strings Trim", "Обрезать Строки", "Trim Strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_TRIM_STRING_ARRAYS", "Trim String Arrays", "Coupez cordes Arrays", "Trim String Array", "Trim String Arrays", "Recorte de Cuerda Arrays", "تقليم سلسلة صفائف", "修剪字符串数组", "文字列の配列をトリミング", "문자열 배열 트림", "Cắt dây Arrays", "Обрезать массивами строк", "Trim String Arrays"));
			Rows.Add( new LocalizationRow("ID_LABEL_ARRAY_DELIMITERS", "Array Delimiters", "Delimiters Array", "Delimitatori Array", "Array Delimiters", "Los delimitadores de matriz", "المحددات مجموعة", "阵列分隔符", "アレイ区切り文字", "배열 구분 기호", "Ký tự phân mảng", "Массив Разделители", "Array Delimiters"));
			Rows.Add( new LocalizationRow("ID_LABEL_NON_STRING", "Non-String", "Non-String", "Non-String", "Non-String", "Non-String", "غير سلسلة", "非字符串", "非文字列", "비 문자열", "Non-String", "Номера строк", "Non-String"));
			Rows.Add( new LocalizationRow("ID_LABEL_STRINGS", "Strings", "Instruments à cordes", "Strings", "Streicher", "Cuerdas", "سلاسل", "字符串", "ストリング", "문자열", "Strings", "Строки", "Strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLEX_TYPES", "Complex Types", "Types complexes", "Tipi complessi", "Komplexe Typen", "Tipos Complejos", "أنواع معقدة", "复杂类型", "複合型", "복합 유형", "Các loại Complex", "Сложные типы", "Complex Types"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLEX_ARRAYS", "Complex Arrays", "Les tableaux complexes", "Array Complex", "Complex Arrays", "Matrices Complejas", "المصفوفات المعقدة", "复杂的阵列", "複雑な配列", "복잡한 배열", "Mảng Complex", "Комплексные массивы", "Complex Arrays"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREATION_OPTIONS", "Creation Options", "Options de création", "Opzioni di creazione", "Erstellungsoptionen", "Opciones de creación", "خيارات خلق", "创建选项", "作成オプション", "창조 옵션", "Tùy chọn tạo", "Опции создания", "Creation Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PLAYMAKER", "Generate Playmaker Actions", "Générer Playmaker Actions", "Genera Playmaker Azioni", "Generieren Spielmacher Aktionen", "Generar acciones Playmaker", "توليد تطبيقات صانع الالعاب", "产生中场组织者的行动", "プレーメーカーのアクションを生成します", "플레이 메이커 작업 생성", "Tạo Playmaker Actions", "Создание плеймейкер действия", "Genereer Playmaker Acties"));
			Rows.Add( new LocalizationRow("ID_LABEL_PERSIST_SCENE_LOADING", "Persist Scene Loading", "Persister Scène de chargement", "Persistere Scene Caricamento", "Persist Szene Loading", "Persistir Escena Cargando", "تستمر المشهد تحميل", "坚持场景加载", "シーンの読み込みを永続化", "장면로드를 지속", "Cố Cảnh tải", "Упорство сцены Загрузка", "Persist Scene Loading"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_FORMATTING", "JSON Formatting Options", "JSON options de formatage", "JSON Opzioni di formattazione", "JSON Formatierungsoptionen", "Opciones de formato JSON", "JSON خيارات التنسيق", "JSON格式选项", "JSONフォーマットのオプション", "JSON 포맷 옵션", "JSON Tùy chọn định dạng", "JSON параметры форматирования", "JSON formatteren Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_EXPORT_CLASS", "Export Generated Class", "Classe Export Création", "Export classe generata", "Export generierte Klassen", "Exportación Clase Generado", "تصدير الفئة التي تم إنشاؤها", "导出生成的类", "エクスポート生成されたクラス", "수출 생성 된 클래스", "Xuất Tạo Lớp", "Экспорт созданного класса", "Export Vernieuwd Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_EXPORT_TYPE", "JSON Export Type", "JSON Type d'exportation", "JSON Export Type", "JSON Export Type", "JSON Tipo Exportación", "JSON نوع التصدير", "JSON导出类型", "JSONエクスポートタイプ", "JSON 내보내기 유형", "JSON khẩu Loại hình", "JSON Экспорт Тип", "JSON Export Type"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_UNICODE", "Escape Unicode Strings", "Échapper Unicode Strings", "Fuga Unicode Strings", "Entfliehen Unicode Strings", "Escapar Unicode Cuerdas", "الهروب يونيكود سلاسل", "逃生的Unicode字符串", "Unicodeの文字列をエスケープします", "유니 코드 문자열을 탈출", "Thoát Unicode Strings", "Побег строк Unicode", "Escape Unicode Strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_CONVERT_CELL_ARRAYS", "Convert Cell Array to String", "Autre réseau de cellules à cordes", "Convertire Array cella a String", "Konvertieren Zellenfeld String", "Convertir matriz celular de cadena", "تحويل صفيف خلية لسلسلة", "单元阵列转换为字符串", "セル配列は、文字列に変換します", "String으로 셀 어레이로 변환", "Chuyển đổi tế bào mảng String", "Преобразование ячеек массива в строку", "Zet Cell Array naar String"));
			Rows.Add( new LocalizationRow("ID_LABEL_VALIDATE_WORKSHEET", "Validate Worksheet", "Valider Feuille", "Convalida Foglio", "Validate Arbeitsblatt", "Validar Hoja de trabajo", "التحقق من صحة ورقة عمل", "验证工作表", "検証ワークシート", "검증 워크 시트", "Validate Worksheet", "Подтвердить лист", "Valideren werkblad"));
			Rows.Add( new LocalizationRow("ID_LABEL_VALIDATE_WORKBOOK", "Validate All Worksheets", "Validez toutes les feuilles", "Convalida tutti i fogli", "Validate Alle Arbeitsblätter", "Validar todas las hojas", "التحقق من صحة جميع أوراق العمل", "验证所有工作表", "すべてのワークシートを検証", "모든 워크 시트의 유효성을 검사합니다", "Xác nhận Tất cả Worksheets", "Подтвердить все листы", "Valideren Alle Werkbladen"));
			Rows.Add( new LocalizationRow("ID_LABEL_CSV_FORMATTING", "CSV Formatting Options", "CSV options de formatage", "CSV Opzioni di formattazione", "CSV Formatierungsoptionen", "Opciones de formato CSV", "CSV خيارات التنسيق", "CSV格式选项", "CSV書式オプション", "CSV 포맷 옵션", "Tùy chọn định dạng CSV", "CSV Опции форматирования", "CSV opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_NGUI_FORMATTING", "NGUI Formatting Options", "Ngui options de formatage", "Ngui Opzioni di formattazione", "Ngui Formatierungsoptionen", "Ngui opciones de formato", "NGUI خيارات التنسيق", "NGUI格式选项", "NGUI書式オプション", "NGUI 포맷 옵션", "Ngừi Tùy chọn định dạng", "NGUI параметры форматирования", "Ngui formatteren Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_STRINGS", "Convert Quotes", "Autre Quotes", "Convertire Quotes", "Konvertieren Quotes", "Convertir Cotizaciones", "تحويل يقتبس", "转换行情", "引用の変換", "지수 변환", "Chuyển đổi Quotes", "Преобразование Цитаты", "Zet Quotes"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_OBJECT_PREVIEW", "Preview JSON Object", "Aperçu JSON objet", "Anteprima JSON oggetto", "Vorschau JSON-Objekt", "Vista previa de JSON objeto", "كائن معاينة JSON", "预览JSON对象", "プレビューJSONオブジェクト", "미리보기 JSON 개체", "Xem trước JSON Object", "Предварительный JSON объект", "Voorbeeld JSON Object"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_CLASS_PREVIEW", "Preview JSON Class", "Aperçu JSON Classe", "Anteprima JSON Class", "Vorschau JSON-Klasse", "Vista previa de JSON Clase", "معاينة الدرجة JSON", "预览JSON类", "プレビューJSONクラス", "미리보기 JSON 클래스", "Xem trước JSON Lớp", "Предварительный JSON Класс", "Voorbeeld JSON Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PREVIEW", "Generate Preview", "Générer Aperçu", "Genera anteprima", "Generiere Vorschau", "Generar Vista previa", "توليد معاينة", "生成预览", "プレビューを生成します", "미리보기를 생성", "Tạo Preview", "Создание просмотр", "Genereer Voorbeeld"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_CLASS", "Generate Class", "Générer Classe", "Generare Classe", "Generieren Class", "Generar Clase", "توليد الفئة", "生成类", "クラスを生成", "클래스를 생성", "Tạo lớp", "Создание класса", "Genereer Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_CSV_PREVIEW", "Preview CSV File", "Aperçu fichier CSV", "Anteprima file CSV", "Vorschau CSV Datei", "Vista previa de un archivo CSV", "معاينة ملف CSV", "预览CSV文件", "プレビューのCSVファイル", "미리보기 CSV 파일", "Xem trước tập tin CSV", "Предварительный CSV-файл", "Voorbeeld CSV-bestand"));
			Rows.Add( new LocalizationRow("ID_LABEL_NGUI_PREVIEW", "Preview NGUI File", "Aperçu Ngui fichier", "Anteprima Ngui File", "Vorschau Ngui Datei", "Prevista ngui Archivo", "معاينة NGUI الملف", "预览NGUI文件", "プレビューNGUIファイル", "미리보기 NGUI 파일", "Xem trước ngừi file", "Предварительный NGUI файла", "Voorbeeld Ngui File"));
			Rows.Add( new LocalizationRow("ID_LABEL_XML_PREVIEW", "Preview XML File", "Aperçu de fichier XML", "Anteprima file XML", "Vorschau XML Datei", "Vista previa de archivos XML", "معاينة ملف XML", "预览XML文件", "プレビューXMLファイル", "미리보기 XML 파일", "Xem trước XML File", "Предварительный XML-файла", "Voorbeeld XML File"));
			Rows.Add( new LocalizationRow("ID_LABEL_XML_FORMATTING", "XML Formatting Options", "XML options de formatage", "Opzioni di formattazione XML", "XML Formatierungsoptionen", "Opciones de formato XML", "خيارات تنسيق XML", "XML格式化选项", "XML書式オプション", "XML 포맷 옵션", "Tùy chọn định dạng XML", "Опции форматирования XML", "XML opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_SHOWONSTARTUP", "Show On Startup", "Montrer Au démarrage", "Mostra All'avvio", "Zeigen On Startup", "Mostrar al arranque", "تظهر عند بدء التشغيل", "在启动时显示", "起動時に表示", "시작에 표시", "Hiện On Startup", "Показать при запуске", "Laat On Startup"));
			Rows.Add( new LocalizationRow("ID_LABEL_CULL_COLUMNS", "Stop On Blank Column", "Arrêtez Sur colonne vide", "Smettere On colonna vuota", "Stoppen Sie auf leere Spalte", "Pare en la columna en blanco", "وقف على عمود فارغ", "停在空白列", "空欄で停止", "빈 열을 중지", "Dừng Ngày Blank Column", "Остановка на пустой колонке", "Stop Op lege kolom"));
			Rows.Add( new LocalizationRow("ID_LABEL_CULL_ROWS", "Stop On Blank Row", "Arrêt sur ligne vide", "Smettere On riga vuota", "Stoppen Sie auf Blank Row", "Pare En Fila En Blanco", "وقف في صف فارغ", "停在空白行", "空白行で停止", "빈 행에 정지", "Dừng Ngày Blank Row", "Остановка на пустой Row", "Stop Op lege rij"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_LINE_BREAKS", "Escape Line Breaks", "Échapper les sauts de ligne", "Fuga interruzioni di riga", "Entfliehen Sie Zeilenumbrüche", "Escapar saltos de línea", "الهروب خط فواصل", "逃脱换行符", "改行をエスケープ", "줄 바꿈을 탈출", "Thoát Ngắt Dòng", "Побег разрывы строк", "Escape Line Breaks"));
			Rows.Add( new LocalizationRow("ID_LABEL_LEGACY_OPTIONS", "Legacy Options", "Options héritées", "Opzioni impostate in precedenza", "Legacy-Optionen", "Opciones adicionales", "خيارات تراث", "传统的选项", "レガシーオプション", "레거시 옵션", "Tùy chọn Legacy", "Дополнительные опции", "Legacy-opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOWERCASE_HEADER", "Make Columns Lowercase", "Assurez-Colonnes Minuscules", "Rendere le colonne minuscola", "Machen Columns Klein", "Hacer Columnas Minúsculas", "جعل الأعمدة حرف صغير", "使列小写", "列を小文字にします", "열 소문자 확인", "Hãy Cột Chữ thường", "Сделать Столбцы Нижний регистр", "Maak Columns kleine letters"));
			Rows.Add( new LocalizationRow("ID_LABEL_TYPEROWBOX_HEADER", "Change Row 2 To Type Row?", "Changer Row 2 au type Row?", "Modifica Row 2 Per tipo di riga?", "Ändern Row 2 Um Zeilentyp?", "Cambio Row 2 para escribir Row?", "تغيير صف 2 لكتابة الصف؟", "更改行2至类型行？", "変更行2は、行を入力するには？", "변경 행 2 행을 입력하려면?", "Thay đổi Row 2 Để Gõ Row?", "Изменить Row 2 Чтобы Введите строку?", "Change Row 2 Om Row Type?"));
			Rows.Add( new LocalizationRow("ID_LABEL_TYPEROWBOX_MESSAGE", "The second row will be converted to a Type row, which will change the entries in your Google Spreadsheet. Are you sure you want to do this?", "La deuxième ligne sera convertie en une ligne Type, qui va changer les entrées dans votre feuille de calcul Google. Êtes-vous sûr de vouloir faire cela?", "La seconda fila sarà convertito in una riga Type, che cambierà le voci nel vostro foglio di calcolo di Google. Sei sicuro di volerlo fare?", "Die zweite Reihe wird zu einer Art Reihe, die die Einträge in Ihrem Google-Tabelle ändern wird umgewandelt werden. Sind Sie sicher, dass Sie dies tun wollen?", "La segunda fila se convertirá en una fila Tipo, que va a cambiar las entradas en su hoja de cálculo de Google. Seguro que quieres hacer esto?", "سيتم تحويل الصف الثاني إلى الصف النوع، والتي سوف تغير الإدخالات في جدول جوجل الخاص بك. هل أنت متأكد أنك تريد أن تفعل هذا؟", "第二排将被转换为一个类型排，这将改变你的谷歌电子表格中的条目。你确定要这么做吗？", "2行目は、Googleスプレッドシートのエントリが変更される、タイプの列に変換されます。あなたはこれを実行してもよろしいですか？", "두 번째 행은 당신의 구글 스프레드 시트의 항목을 변경하는 유형의 행으로 변환됩니다. 당신은이 작업을 수행 하시겠습니까?", "Hàng thứ hai sẽ được chuyển đổi sang một dòng Type, mà sẽ thay đổi các mục trong bảng tính Google của bạn. Bạn có chắc chắn muốn làm điều này?", "Второй ряд будет преобразован в тип строки, которая будет изменить записи в таблицы Google. Вы уверены, что хотите это сделать?", "De tweede rij zal worden omgezet naar een Type rij, waarin de items in uw Google Spreadsheet zal veranderen. Bent u zeker dat u dit wilt doen?"));
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
		public LocalizationRow GetRow(rowIds in_RowID)
		{
			LocalizationRow ret = null;
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
		public LocalizationRow GetRow(string in_RowString)
		{
			LocalizationRow ret = null;
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
