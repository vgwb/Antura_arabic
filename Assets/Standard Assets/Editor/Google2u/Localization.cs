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
			, ID_LABEL_CULL_ROWS, ID_LABEL_ESCAPE_LINE_BREAKS, ID_LABEL_LEGACY_OPTIONS, ID_LABEL_LOWERCASE_HEADER, ID_LABEL_TYPEROWBOX_HEADER, ID_LABEL_TYPEROWBOX_MESSAGE, ID_LABEL_CODE_GENERATION_OPTIONS, ID_LABEL_PREPEND_UNDERSCORES, ID_LABEL_XML_COLUMN_AS_CHILD_TAGS
		};
		public string [] rowNames = {
			"ID_ERROR_EMPTY_URL", "ID_ERROR_INVALID_URL", "ID_ERROR_INVALID_RESOURCES_DIR", "ID_ERROR_INVALID_EDITOR_DIR", "ID_ERROR_INVALID_DIR", "ID_ERROR_INVALID_UPLOAD_DIR", "ID_ERROR_BUILD_TARGET", "ID_ERROR_INVALID_WORKSHEET", "ID_MESSAGE_QUERYING_WORKBOOKS", "ID_MESSAGE_QUERYING_WORKSHEETS", "ID_MESSAGE_QUERYING_CELLS", "ID_MESSAGE_REMOVE_WORKBOOK", "ID_MESSAGE_NO_WORKSHEETS", "ID_MESSAGE_EMPTY_WORKSHEET", "ID_MESSAGE_LOGGED_IN_AS", "ID_MESSAGE_PROCESSING_LOGIN", "ID_MESSAGE_RETRIEVING_WORKBOOKS", "ID_MESSAGE_UPLOADING_WORKBOOK", "ID_MESSAGE_PLAY_MODE"
			, "ID_LABEL_ACTIVE_WORKSHEET", "ID_LABEL_EXPORT_AS", "ID_LABEL_OPEN_IN_GOOGLE", "ID_LABEL_REFRESH_WORKBOOK", "ID_LABEL_EDIT_WORKBOOK", "ID_LABEL_VIEW_WORKBOOK", "ID_LABEL_REMOVE_WORKBOOK", "ID_LABEL_RELOAD_WORKBOOKS", "ID_LABEL_DELETE", "ID_LABEL_CANCEL", "ID_LABEL_EXPORT", "ID_LABEL_LOGIN", "ID_LABEL_LOGOUT", "ID_LABEL_CREDENTIALS", "ID_LABEL_SAVE_CREDENTIALS", "ID_LABEL_AUTO_LOGIN", "ID_LABEL_USERNAME", "ID_LABEL_PASSWORD", "ID_LABEL_SETTINGS", "ID_LABEL_LANGUAGE"
			, "ID_LABEL_EDITOR_LANGUAGE", "ID_LABEL_EXPORTERS", "ID_LABEL_GENERATE_PATHS", "ID_LABEL_ENABLE", "ID_LABEL_GAME_OBJECT_DATABASE", "ID_LABEL_OBJECT_DATABASE", "ID_LABEL_STATIC_DATABASE", "ID_LABEL_RESOURCES_DIR", "ID_LABEL_EDITOR_DIR", "ID_LABEL_EXPORT_DIR", "ID_LABEL_CHOOSE_FOLDER", "ID_LABEL_WORKBOOKS", "ID_LABEL_MANUAL_WORKBOOKS", "ID_LABEL_ACCOUNT_WORKBOOKS", "ID_LABEL_ADD_WORKBOOK", "ID_LABEL_UPLOAD_WORKBOOK", "ID_LABEL_COMPLETE", "ID_LABEL_CHOOSE_FILE", "ID_LABEL_SELECT_WORKBOOK_PATH", "ID_LABEL_HELP"
			, "ID_LABEL_CONTACT", "ID_LABEL_BROWSE_LITTERATUS", "ID_LABEL_BROWSE_UNITY", "ID_LABEL_CREATED_WITH_UNITY", "ID_LABEL_COPYRIGHT_UNITY", "ID_LABEL_DOCUMENTATION", "ID_LABEL_UPDATE", "ID_LABEL_SYNC", "ID_LABEL_EXPORT_OPTIONS", "ID_LABEL_WHITESPACE", "ID_LABEL_TRIM_STRINGS", "ID_LABEL_TRIM_STRING_ARRAYS", "ID_LABEL_ARRAY_DELIMITERS", "ID_LABEL_NON_STRING", "ID_LABEL_STRINGS", "ID_LABEL_COMPLEX_TYPES", "ID_LABEL_COMPLEX_ARRAYS", "ID_LABEL_CREATION_OPTIONS", "ID_LABEL_GENERATE_PLAYMAKER", "ID_LABEL_PERSIST_SCENE_LOADING"
			, "ID_LABEL_JSON_FORMATTING", "ID_LABEL_JSON_EXPORT_CLASS", "ID_LABEL_JSON_EXPORT_TYPE", "ID_LABEL_ESCAPE_UNICODE", "ID_LABEL_CONVERT_CELL_ARRAYS", "ID_LABEL_VALIDATE_WORKSHEET", "ID_LABEL_VALIDATE_WORKBOOK", "ID_LABEL_CSV_FORMATTING", "ID_LABEL_NGUI_FORMATTING", "ID_LABEL_ESCAPE_STRINGS", "ID_LABEL_JSON_OBJECT_PREVIEW", "ID_LABEL_JSON_CLASS_PREVIEW", "ID_LABEL_GENERATE_PREVIEW", "ID_LABEL_GENERATE_CLASS", "ID_LABEL_CSV_PREVIEW", "ID_LABEL_NGUI_PREVIEW", "ID_LABEL_XML_PREVIEW", "ID_LABEL_XML_FORMATTING", "ID_LABEL_SHOWONSTARTUP", "ID_LABEL_CULL_COLUMNS"
			, "ID_LABEL_CULL_ROWS", "ID_LABEL_ESCAPE_LINE_BREAKS", "ID_LABEL_LEGACY_OPTIONS", "ID_LABEL_LOWERCASE_HEADER", "ID_LABEL_TYPEROWBOX_HEADER", "ID_LABEL_TYPEROWBOX_MESSAGE", "ID_LABEL_CODE_GENERATION_OPTIONS", "ID_LABEL_PREPEND_UNDERSCORES", "ID_LABEL_XML_COLUMN_AS_CHILD_TAGS"
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
			Rows.Add( new LocalizationRow("ID_ERROR_EMPTY_URL", "The URL is empty", "L'URL est vide", "L'URL è vuota", "Die URL ist leer", "La URL está vacía", "عنوان URL فارغ", "网址是空的", "URLは空です", "URL은 비어 있습니다", "Các URL trống", "URL пуст", "De URL is leeg"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_URL", "The URL is invalid", "L'URL est invalide", "L'URL non è valido", "Die URL ist ungültig", "La URL no es válida", "عنوان URL غير صالح", "网址无效", "URLが無効です", "URL이 잘못되었습니다", "Các URL không hợp lệ", "URL является недействительным", "De URL is ongeldig"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_RESOURCES_DIR", "You must choose an resources directory within this project", "Vous devez choisir un répertoire de ressources au sein de ce projet", "È necessario scegliere una directory delle risorse all'interno di questo progetto", "Sie müssen ein Ressourcen-Verzeichnis in diesem Projekt wählen", "Debe elegir un directorio de recursos dentro de este proyecto", "يجب عليك اختيار دليل الموارد ضمن هذا المشروع", "必须选择该项目内的资源目录", "あなたはこのプロジェクト内のリソース・ディレクトリーを選択する必要があります", "이 프로젝트 내에서 자원 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục nguồn lực trong dự án này", "Вы должны выбрать каталог ресурсов в рамках данного проекта", "U moet kiezen voor een directory middelen binnen dit project"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_EDITOR_DIR", "You must choose an editor directory within this project", "Vous devez choisir un répertoire de l'éditeur dans ce projet", "È necessario scegliere una cartella editore all'interno di questo progetto", "Sie müssen einen Editor Verzeichnis innerhalb dieses Projekt wählen", "Debe elegir un directorio editor dentro de este proyecto", "يجب عليك اختيار دليل محرر ضمن هذا المشروع", "必须选择该项目中的编辑目录", "あなたはこのプロジェクト内のエディタのディレクトリを選択する必要があります", "이 프로젝트에서 에디터 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục biên tập viên trong dự án này", "Вы должны выбрать директорию редактора в рамках данного проекта", "Je moet kiezen een editor directory binnen dit project"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_DIR", "You must choose a directory within this project", "Vous devez choisir un répertoire dans ce projet", "È necessario scegliere una directory all'interno di questo progetto", "Sie müssen ein Verzeichnis in diesem Projekt wählen", "Debe elegir un directorio dentro de este proyecto", "يجب عليك اختيار الدليل ضمن هذا المشروع", "必须选择该项目中的目录", "あなたはこのプロジェクト内のディレクトリを選択する必要があります", "이 프로젝트 내에서 디렉토리를 선택해야합니다", "Bạn phải chọn một thư mục trong dự án này", "Вы должны выбрать директорию в рамках данного проекта", "U moet een directory binnen dit project te kiezen"));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_UPLOAD_DIR", "Workbook Upload Path Invalid", "Workbook Upload Chemin non valide", "Cartella di lavoro Carica Percorso non valido", "Arbeitsmappe Hochladen Pfad ungültig", "Libro de trabajo Cargar ruta no válida", "المصنف تحميل مسار غير صالح", "工作簿上传路径无效", "ワークブックのアップロードパスが無効です。", "통합 문서 업로드 경로가 잘못되었습니다", "Workbook Tải lên Đường dẫn không hợp lệ", "Рабочая тетрадь загрузки Путь Invalid", "Werkboek uploaden Path ongeldig"));
			Rows.Add( new LocalizationRow("ID_ERROR_BUILD_TARGET", "Google2u is unable to communicate with Google using this build target. Please switch to either the Standalone or Mobile targets.", "Google2u est incapable de communiquer avec Google en utilisant cette cible de construction. S'il vous plaît passer soit à l'autonome ou des cibles mobiles.", "Google2u non è in grado di comunicare con Google con questo target build. Si prega di passare a uno il Standalone o bersagli mobili.", "Google2u kann nicht mit Google mit diesem Build-Ziel zu kommunizieren. Schalten Sie bitte entweder auf den Standalone oder bewegliche Ziele.", "Google2u es incapaz de comunicarse con Google el uso de este tipo de generación. Por favor, utilice la ficha Independiente o blancos móviles.", "Google2u غير قادر على التواصل مع Google باستخدام هذا البناء المستهدف. يرجى التبديل إلى إما بذاتها أو أهداف المحمول.", "Google2u是无法与谷歌使用此构建目标进行通信。请切换到无论是独立或移动目标。", "Google2uは、このビルドターゲットを使用してGoogleと通信することができません。スタンドアロンまたはモバイルターゲットのいずれかに切り替えてください。", "Google2u 구글이 빌드 타겟을 이용하여 통신 할 수 없습니다. 독립 또는 모바일 대상 중 하나로 전환하십시오.", "Google2u là không thể giao tiếp với Google sử dụng xây dựng được mục tiêu này. Hãy chuyển sang một trong hai độc hoặc các mục tiêu di động.", "Google2u не может связаться с Google с помощью этой цели сборки. Пожалуйста, переключиться в режим автономного или подвижных целей.", "Google2u niet in staat is om te communiceren met Google het gebruik van deze build doel. Schakel om ofwel de Standalone of Mobile doelen."));
			Rows.Add( new LocalizationRow("ID_ERROR_INVALID_WORKSHEET", "Invalid Worksheet", "Feuille non valide", "foglio di lavoro non valido", "Ungültige Arbeitsblatt", "Hoja de trabajo no válido", "ورقة عمل غير صالح", "无效的工作表", "無効なワークシート", "잘못된 워크 시트", "Worksheet không hợp lệ", "Invalid Рабочий лист", "ongeldige werkblad"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_WORKBOOKS", "Querying Workbooks", "Interrogation Workbooks", "Interrogazione cartelle di lavoro", "Querying Workbooks", "Los libros de consulta", "الاستعلام عن المصنفات", "查询工作簿", "問合せワークブック", "쿼리 통합 문서", "Truy vấn Workbooks", "Запрос Workbooks", "querying werkmappen"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_WORKSHEETS", "Querying Worksheets", "Interrogation Worksheets", "Interrogazione fogli di lavoro", "Querying Arbeitsblätter", "consulta Hojas de trabajo", "الاستعلام عن أوراق العمل", "查询工作表", "問合せワークシート", "쿼리 워크 시트", "Truy vấn Worksheets", "Запрос Worksheets", "querying Werkbladen"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_QUERYING_CELLS", "Querying Cells", "Interrogation cellules", "Interrogazione Cells", "Abfragen von Zellen", "Consulta de células", "الاستعلام عن الخلايا", "查询细胞", "細胞の照会", "세포 쿼리", "Truy vấn Cells", "Cells Выполнение запросов", "bevragen Cells"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_REMOVE_WORKBOOK", "Remove the Manual Workbook? This cannot be undone.", "Retirez le classeur manuel? Ça ne peut pas être annulé.", "Rimuovere la cartella di lavoro manuale? Questo non può essere annullata.", "Entfernen Sie die manuelle Arbeitsmappe? Das kann nicht rückgängig gemacht werden.", "Retire el libro de trabajo manual? Esto no se puede deshacer.", "إزالة اليدوي المصنف؟ هذا لا يمكن التراجع عنها.", "取下手动工作簿？这不能被撤消。", "マニュアルワークブックを削除しますか？これは、元に戻すことはできません。", "수동 통합 문서를 삭제 하시겠습니까? 이 취소 할 수 없습니다.", "Tháo Workbook tay? Điều này không thể được hoàn tác.", "Удалить ручной рабочей книги? Это не может быть отменено.", "Verwijder de handmatige Werkboek? Dit kan niet ongedaan gemaakt worden."));
			Rows.Add( new LocalizationRow("ID_MESSAGE_NO_WORKSHEETS", "No Worksheets Found", "Aucun Worksheets trouvés", "Non ci sono fogli di lavoro Trovato", "Keine gefunden Worksheets", "No se han encontrado hojas de trabajo", "لا أوراق العمل تم العثور عليها", "没有找到工作表", "いいえワークシートが見つかりません", "어떤 워크 시트를 찾을 수 없습니다", "Không Worksheets Tìm thấy", "Нет Worksheets не найдено", "Geen Werkbladen gevonden"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_EMPTY_WORKSHEET", "Empty Worksheet", "Feuille vide", "foglio di lavoro vuoto", "leeres Arbeitsblatt", "Hoja de trabajo vacía", "ورقة عمل فارغة", "空表", "空のワークシート", "빈 워크 시트", "Worksheet trống", "Пустой лист", "leeg werkblad"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_LOGGED_IN_AS", "Logged In", "Connecté", "Connesso", "Eingeloggt", "Conectado", "تسجيل الدخول", "登录", "ログインして", "로그인", "Đăng nhập", "Записан В", "Ingelogd"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_PROCESSING_LOGIN", "Processing Login, Please Wait", "Traitement Connexion, S'il vous plaît attendre", "Processing Login, Attendere prego", "Die Verarbeitung Anmeldung, bitte warten", "Procesamiento de inicio de sesión, por favor espere", "معالجة الدخول، الرجاء الانتظار", "处理登录，请稍候", "ログインを処理して、お待ちください", "가공 로그인, 잠시 기다려주십시오", "Xử lý đăng nhập, Please Wait", "Обработка Войти, пожалуйста, подождите", "Processing Login, Please Wait"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_RETRIEVING_WORKBOOKS", "Retrieving Workbooks, Please Wait", "Récupération Classeurs, S'il vous plaît Attendez", "Recupero di cartelle di lavoro, Attendere prego", "Abrufen von Arbeitsmappen, bitte warten", "Recuperando libros, por favor, espere", "استرجاع المصنفات، من فضلك انتظر", "检索工作簿，请稍候", "ワークブックの取得、お待ちください", "통합 문서를 가져 오는 중, 잠시 기다려주십시오", "Lấy Workbooks, Please Wait", "Получение рабочих книг, пожалуйста, подождите", "Ophalen van werkboeken, Please Wait"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_UPLOADING_WORKBOOK", "Uploading Workbook", "Uploading Workbook", "Caricamento cartella di lavoro", "Das Hochladen der Arbeitsmappe", "Carga de Libro de Trabajo", "تحميل مصنف", "上传工作簿", "アップロードワークブック", "업로드 통합 문서", "Tải lên Workbook", "Выгрузка Рабочая тетрадь", "uploaden Werkboek"));
			Rows.Add( new LocalizationRow("ID_MESSAGE_PLAY_MODE", "Google2u is an Editor-Only application. Functionality is unavailable in Play Mode.", "Google2u est une application Editor-Only. La fonctionnalité est indisponible en mode Play.", "Google2u è un'applicazione Editor-Only. La funzionalità non è disponibile in modalità Play.", "Google2u ist ein Editor-Only-Anwendung. Die Funktionalität ist im Play-Modus nicht verfügbar.", "Google2u es una aplicación de edición de sólo. Funcionalidad no está disponible en el modo de reproducción.", "Google2u هو تطبيق محرر فقط. وظائف غير متوفرة في طريقة اللعب.", "Google2u是主编唯一应用程序。功能在播放模式下无法使用。", "Google2uは、エディタ専用のアプリケーションです。機能は、再生モードでは使用できません。", "Google2u은 에디터 전용 응용 프로그램입니다. 기능은 재생 모드에서 사용할 수 없습니다.", "Google2u là một ứng dụng biên tập-Only. Chức năng là không có sẵn trong chế độ Play.", "Google2u является редактором только приложение. Функциональность недоступна в режиме воспроизведения.", "Google2u is een Editor-Only applicatie. Functionaliteit is niet beschikbaar in de Play Mode."));
			Rows.Add( new LocalizationRow("ID_LABEL_ACTIVE_WORKSHEET", "Active Worksheet", "Feuille active", "foglio di lavoro attivo", "Aktive Arbeitsblatt", "hoja de cálculo activa", "ورقة عمل نشطة", "活动工作表", "アクティブワークシート", "활성 워크 시트", "hoạt động Worksheet", "Активный Рабочий лист", "actieve werkblad"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_AS", "Export as", "Exporter sous forme de", "Esporta come", "Export als", "Exportar como", "التصدير،", "作为出口", "エクスポート", "로 내보내기", "Xuất khẩu như", "Экспорт в", "Exporteren als"));
			Rows.Add( new LocalizationRow("ID_LABEL_OPEN_IN_GOOGLE", "Open Workbook in Google", "Ouvrir un classeur dans Google", "Apri cartella di lavoro in Google", "Arbeitsmappe öffnen in Google", "Libro abierto en Google", "مصنف مفتوح في جوجل", "在谷歌打开工作簿", "グーグルでワークブックを開きます", "구글에서 열기 통합 문서", "Mở Workbook trong Google", "Открыть книгу в Google", "Open Werkboek in Google"));
			Rows.Add( new LocalizationRow("ID_LABEL_REFRESH_WORKBOOK", "Refresh Workbook", "Refresh Workbook", "Aggiorna cartella di lavoro", "Refresh-Arbeitsmappe", "Actualizar Libro de Trabajo", "تحديث مصنف", "刷新工作簿", "リフレッシュワークブック", "새로 고침 통합 문서", "Refresh Workbook", "Обновить Рабочая тетрадь", "Vernieuwen Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDIT_WORKBOOK", "Edit Workbook", "Modifier le classeur", "Modifica cartella di lavoro", "Bearbeiten Arbeitsmappe", "Editar Libro de Trabajo", "تحرير مصنف", "编辑工作簿", "編集ワークブック", "편집 통합 문서", "Sửa Workbook", "Редактировать Рабочая тетрадь", "bewerken Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_VIEW_WORKBOOK", "View Workbook", "Voir le classeur", "Visualizza cartella di lavoro", "Ansicht Arbeitsmappe", "Ver Libro de Trabajo", "عرض مصنف", "查看工作簿", "ビューワークブック", "보기 통합 문서", "Xem Workbook", "Просмотр Рабочая тетрадь", "Bekijk Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_REMOVE_WORKBOOK", "Remove Workbook", "Retirer le classeur", "rimuovere cartella di lavoro", "entfernen Arbeitsmappe", "documento de eliminaciones", "إزالة مصنف", "删除工作簿", "ワークブックを削除します", "통합 문서를 제거", "Di Workbook", "Удалить Workbook", "Verwijder Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_RELOAD_WORKBOOKS", "Reload Workbooks", "Recharger Workbooks", "Ricarica cartelle di lavoro", "Reload Workbooks", "Volver a cargar libros", "تحديث المصنفات", "刷新工作簿", "リロードワークブック", "새로 고침 통합 문서", "Nạp lại Workbooks", "Перезагрузить Рабочая тетрадь", "Reload werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_DELETE", "Delete", "Effacer", "cancellare", "Löschen", "Borrar", "حذف", "删除", "削除", "지우다", "Xóa bỏ", "Удалить", "Verwijder"));
			Rows.Add( new LocalizationRow("ID_LABEL_CANCEL", "Cancel", "Annuler", "Annulla", "Stornieren", "Cancelar", "إلغاء", "取消", "キャンセル", "취소하다", "hủy bỏ", "Отмена", "Annuleer"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT", "Export", "Exportation", "Esportare", "Export", "Exportar", "تصدير", "出口", "輸出する", "수출", "Xuất khẩu", "экспорт", "Exporteren"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOGIN", "Log In", "S'identifier", "Accesso", "Einloggen", "Iniciar sesión", "تسجيل الدخول", "登录", "ログイン", "로그인", "Đăng nhập", "Авторизоваться", "Log in"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOGOUT", "Log Out", "Se déconnecter", "Disconnettersi", "Ausloggen", "Cerrar sesión", "خروج", "登出", "ログアウト", "로그 아웃", "Đăng xuất", "Выйти", "Uitloggen"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREDENTIALS", "Credentials", "Lettres de créance", "Credenziali", "Referenzen", "Cartas credenciales", "أوراق اعتماد", "证书", "資格情報", "신임장", "Thông tin đăng nhập", "полномочия", "Geloofsbrieven"));
			Rows.Add( new LocalizationRow("ID_LABEL_SAVE_CREDENTIALS", "Save Credentials", "Enregistrer de vérification des pouvoirs", "Salva credenziali", "speichern Credentials", "guardar credenciales", "حفظ وثائق التفويض", "保存凭证", "資格情報を保存", "자격 증명을 저장", "lưu Credentials", "Сохранить учетные данные", "Save geloofsbrieven"));
			Rows.Add( new LocalizationRow("ID_LABEL_AUTO_LOGIN", "Auto Login", "Connexion automatique", "Login automatico", "Auto Login", "Ingreso automático", "تسجيل تلقائى", "自动登录", "自動ログイン", "자동 로그인", "Tự động đăng nhập", "Автоматическая авторизация", "Automatische login"));
			Rows.Add( new LocalizationRow("ID_LABEL_USERNAME", "User Name", "Nom d'utilisateur", "Nome utente", "Benutzername", "Nombre de usuario", "اسم المستخدم", "用户名", "ユーザー名", "사용자 이름", "tên người dùng", "имя пользователя", "Gebruikersnaam"));
			Rows.Add( new LocalizationRow("ID_LABEL_PASSWORD", "Password", "Mot de passe", "parola d'ordine", "Passwort", "Contraseña", "كلمه السر", "密码", "パスワード", "암호", "Mật khẩu", "пароль", "Wachtwoord"));
			Rows.Add( new LocalizationRow("ID_LABEL_SETTINGS", "Settings", "Paramètres", "impostazioni", "Einstellungen", "ajustes", "إعدادات", "设置", "設定", "설정", "Cài đặt", "настройки", "instellingen"));
			Rows.Add( new LocalizationRow("ID_LABEL_LANGUAGE", "Language", "La langue", "Lingua", "Sprache", "Idioma", "لغة", "语言", "言語", "언어", "ngôn ngữ", "язык", "Taal"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDITOR_LANGUAGE", "Editor Language", "Editor Langue", "Editor Lingua", "Editor Sprache", "editor de idiomas", "محرر لغة", "编辑语言", "エディタの言語", "에디터 언어", "Biên tập viên Ngôn ngữ", "Редактор языка", "editor Taal"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORTERS", "Export Paths", "Export Chemins", "Export Percorsi", "Export-Pfade", "Caminos de exportación", "تصدير مسارات", "导出路径", "エクスポートパス", "수출 경로", "Xuất Paths", "Экспорт Дорожки", "export Paths"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PATHS", "Generate Default Paths", "Générer des chemins par défaut", "Genera percorsi predefiniti", "Generieren Sie Standardpfade", "Generar rutas predeterminadas", "توليد مسارات افتراضي", "生成默认路径", "デフォルトのパスを生成します", "기본 경로를 생성", "Tạo Mặc định đường dẫn", "Генерация по умолчанию Дорожки", "Genereer Default Paths"));
			Rows.Add( new LocalizationRow("ID_LABEL_ENABLE", "Enable", "Activer", "consentire", "Aktivieren", "Habilitar", "تمكين", "启用", "有効にします", "사용", "cho phép", "включить", "in staat stellen"));
			Rows.Add( new LocalizationRow("ID_LABEL_GAME_OBJECT_DATABASE", "Game Object Database", "Jeu d'objets de base de données", "Gioco di oggetti di database", "Spiel Object Database", "Base de datos de objetos de juego", "قاعدة بيانات اللعبة وجوه", "游戏对象数据库", "ゲームのオブジェクトデータベース", "게임 오브젝트 데이터베이스", "Cơ sở dữ liệu đối tượng trò chơi", "База данных объектов игры", "Game Object Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_OBJECT_DATABASE", "Object Database", "Base de données de l'objet", "Database Object", "Objektdatenbank", "Base de datos de objetos", "قاعدة بيانات كائن", "对象数据库", "オブジェクトデータベース", "객체 데이터베이스", "Cơ sở dữ liệu đối tượng", "База данных объектов", "object Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_STATIC_DATABASE", "Static Database", "Base de données statique", "Database statico", "Statische Datenbank", "base de datos estática", "قاعدة بيانات ثابتة", "静态数据库", "静的データベース", "정적 데이터베이스", "Cơ sở dữ liệu tĩnh", "Статические базы данных", "statische Database"));
			Rows.Add( new LocalizationRow("ID_LABEL_RESOURCES_DIR", "Resources Directory", "Répertoire des ressources", "risorse Directory", "Ressourcen-Verzeichnis", "Directorio de recursos", "دليل الموارد", "资源目录", "リソースディレクトリ", "자원 디렉토리", "Tài mục", "Каталог ресурсов", "Resources Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_EDITOR_DIR", "Editor Directory", "Directory Editor", "Directory Editor", "Editor-Verzeichnis", "Directory editor", "دليل محرر", "编辑目录", "エディタのディレクトリ", "에디터 디렉토리", "Biên tập viên mục", "Редактор каталогов", "editor Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_DIR", "Export Directory", "Directory Export", "Directory Export", "Exportverzeichnis", "Exportación de directorio", "دليل الصادرات", "导出目录", "エクスポートディレクトリ", "수출 디렉토리", "Thư mục Xuất", "Экспорт Каталог", "export Directory"));
			Rows.Add( new LocalizationRow("ID_LABEL_CHOOSE_FOLDER", "Choose a Folder", "Choisissez un dossier", "Scegliere una cartella", "Wählen Sie einen Ordner", "Elija una carpeta", "اختيار مجلد", "选择一个文件夹", "フォルダを選択します。", "폴더를 선택", "Chọn một thư mục", "Выберите папку", "Kies een map"));
			Rows.Add( new LocalizationRow("ID_LABEL_WORKBOOKS", "Workbooks", "classeurs", "Le cartelle di lavoro", "Workbooks", "Libros de Trabajo", "المصنفات", "工作簿", "ワークブック", "통합 문서", "Workbooks", "Учебные пособия", "werkboeken"));
			Rows.Add( new LocalizationRow("ID_LABEL_MANUAL_WORKBOOKS", "Manual Workbooks", "classeurs Manuel", "Le cartelle di lavoro manuale", "Manuelle Arbeitsmappen", "Los libros manuales", "المصنفات اليدوية", "手动工作簿", "マニュアルワークブック", "수동 통합 문서", "Workbooks tay", "Руководство Workbooks", "Manual werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_ACCOUNT_WORKBOOKS", "Account Workbooks", "compte Workbooks", "conto cartelle di lavoro", "Konto Workbooks", "cuenta libros", "المصنفات حساب", "帐户工作簿", "アカウントワークブック", "계정 통합 문서", "Workbooks tài khoản", "Счет Workbooks", "account werkmappen"));
			Rows.Add( new LocalizationRow("ID_LABEL_ADD_WORKBOOK", "Add Manual Workbook", "Ajouter le classeur Manuel", "Aggiungi cartella di lavoro manuale", "In Manuelle Arbeitsmappe", "Añadir libro de trabajo manual", "إضافة مصنف يدوي", "添加手动工作簿", "マニュアルブックを追加", "수동 통합 문서 추가", "Thêm Workbook tay", "Добавить Manual Workbook", "Voeg Manual Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_UPLOAD_WORKBOOK", "Upload Workbook", "Télécharger Workbook", "Carica cartella di lavoro", "hochladen Arbeitsmappe", "Subir Libro de Trabajo", "تحميل مصنف", "上传工作簿", "アップロードワークブック", "업로드 통합 문서", "Tải lên Workbook", "Загрузить Рабочая тетрадь", "Upload Werkboek"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLETE", "Complete", "Achevée", "Completare", "Komplett", "Completar", "كامل", "完成", "コンプリート", "완전한", "Hoàn thành", "полный", "compleet"));
			Rows.Add( new LocalizationRow("ID_LABEL_CHOOSE_FILE", "Choose a File to Upload", "Choisissez un fichier à télécharger", "Scegli un file da caricare", "Wählen Sie eine Datei zum hochladen", "Elija un archivo para subir", "اختيار ملف لتحميل", "选择要上传的文件", "アップロードするファイルを選択します。", "업로드 할 파일을 선택합니다", "Chọn một tập tin để tải", "Выберите файл для загрузки", "Kies een bestand te uploaden"));
			Rows.Add( new LocalizationRow("ID_LABEL_SELECT_WORKBOOK_PATH", "Select Workbook Path", "Sélectionnez Workbook Chemin", "Seleziona cartella di lavoro Path", "Wählen Sie Arbeitspfads", "Seleccionar libro Camino", "حدد مصنف مسار", "选择工作簿路径", "ワークブックのパスを選択します", "통합 문서 경로를 선택", "Chọn Workbook Đường dẫn", "Выберите Workbook Путь", "Selecteer Werkboek Path"));
			Rows.Add( new LocalizationRow("ID_LABEL_HELP", "Help", "Aidez-moi", "Aiuto", "Hilfe", "Ayuda", "مساعدة", "帮帮我", "助けて", "도움", "Cứu giúp", "Помогите", "Hulp"));
			Rows.Add( new LocalizationRow("ID_LABEL_CONTACT", "Contact", "Contact", "contatto", "Kontakt", "Contacto", "اتصال", "联系", "接触", "접촉", "Tiếp xúc", "контакт", "Contact"));
			Rows.Add( new LocalizationRow("ID_LABEL_BROWSE_LITTERATUS", "Browse to Litteratus.net", "Accédez à Litteratus.net", "Individuare Litteratus.net", "Suchen Sie nach Litteratus.net", "Vaya a Litteratus.net", "استعرض للوصول إلى Litteratus.net", "浏览到Litteratus.net", "Litteratus.netを参照", "Litteratus.net로 이동", "Duyệt đến Litteratus.net", "Перейдите к Litteratus.net", "Blader naar Litteratus.net"));
			Rows.Add( new LocalizationRow("ID_LABEL_BROWSE_UNITY", "Browse to Unity3d.com", "Accédez à Unity3d.com", "Individuare Unity3d.com", "Suchen Sie nach Unity3d.com", "Vaya a Unity3d.com", "استعرض للوصول إلى Unity3d.com", "浏览到Unity3d.com", "Unity3d.comを参照", "Unity3d.com로 이동", "Duyệt đến Unity3d.com", "Перейдите к Unity3d.com", "Blader naar Unity3d.com"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREATED_WITH_UNITY", "Created with Unity", "Créé avec l'unité", "Creato con Unity", "Erstellt mit Unity", "Creado con Unity", "تم إنشاؤها مع الوحدة", "创建使用Unity", "Unityと作成", "유니티로 만든", "Tạo ra với Unity", "Создано с Unity", "Gemaakt met Unity"));
			Rows.Add( new LocalizationRow("ID_LABEL_COPYRIGHT_UNITY", "Copyright 2014", "Droit d'auteur 2014", "Copyright 2014", "Copyright 2014", "Derechos de autor 2014", "حقوق التأليف والنشر 2014", "2014年版权所有", "著作権2014", "저작권 2014", "Bản quyền 2014", "Copyright 2014", "Copyright 2014"));
			Rows.Add( new LocalizationRow("ID_LABEL_DOCUMENTATION", "Documentation", "Documentation", "Documentazione", "Dokumentation", "Documentación", "توثيق", "文档", "ドキュメンテーション", "선적 서류 비치", "Tài liệu", "Документация", "Documentatie"));
			Rows.Add( new LocalizationRow("ID_LABEL_UPDATE", "Update", "Mettre à jour", "Aggiornare", "Aktualisieren", "Actualizar", "تحديث", "更新", "更新", "최신 정보", "cập nhật", "Обновить", "Bijwerken"));
			Rows.Add( new LocalizationRow("ID_LABEL_SYNC", "Sync with Google", "Synchronisation avec Google", "Sincronizzazione con Google", "Synchronisierung mit Google", "Sincronizar con Google", "مزامنة مع جوجل", "与谷歌同步", "Googleとの同期", "구글 동기화", "Đồng bộ hóa với Google", "Синхронизация с помощью Google", "Synchroniseren met Google"));
			Rows.Add( new LocalizationRow("ID_LABEL_EXPORT_OPTIONS", "Export Options", "Options d'exportation", "Opzioni di esportazione", "Exportoptionen", "Opciones de exportación", "خيارات التصدير", "导出选项", "エクスポートオプション", "내보내기 옵션", "Tùy chọn Export", "параметры экспорта", "export Options"));
			Rows.Add( new LocalizationRow("ID_LABEL_WHITESPACE", "Whitespace", "whitespace", "Spazio bianco", "Leer", "Los espacios en blanco", "الفضاء الابيض", "空白", "空白", "공백", "Khoảng trắng", "Пробелы", "Witte ruimte"));
			Rows.Add( new LocalizationRow("ID_LABEL_TRIM_STRINGS", "Trim Strings", "Cordes Garniture", "Strings Trim", "Trim Strings", "Cuerdas de acabado", "سلاسل تقليم", "修剪字符串", "トリム文字列", "트림 문자열", "Strings Trim", "Обрезка Струны", "Trim Strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_TRIM_STRING_ARRAYS", "Trim String Arrays", "Coupez cordes Arrays", "Trim String Array", "Trim String-Arrays", "Recorte de Cuerda matrices", "تقليم سلسلة صفائف", "修剪字符串数组", "文字列配列のトリム", "문자열 배열 트림", "Cắt Chuỗi Mảng", "Обрежьте массивами строк", "Trim String Arrays"));
			Rows.Add( new LocalizationRow("ID_LABEL_ARRAY_DELIMITERS", "Array Delimiters", "Delimiters Array", "delimitatori Array", "Array Trenner", "Los delimitadores de matriz", "المحددات مجموعة", "阵列分隔符", "アレイ区切り文字", "배열 구분 기호", "ký tự phân mảng", "Массив Разделители", "Array Delimiters"));
			Rows.Add( new LocalizationRow("ID_LABEL_NON_STRING", "Non-String", "Non-String", "Non-String", "Non-String", "Que no son cadenas", "غير سلسلة", "非字符串", "非文字列", "비 문자열", "Non-String", "Нестроковых", "Non-String"));
			Rows.Add( new LocalizationRow("ID_LABEL_STRINGS", "Strings", "instruments à cordes", "archi", "Streicher", "Instrumentos de cuerda", "سلاسل", "字符串", "ストリング", "문자열", "Dây", "Строки", "strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLEX_TYPES", "Complex Types", "Types complexes", "tipi complessi", "Komplexe Typen", "Tipos complejos", "أنواع معقدة", "复杂类型", "複合型", "복합 유형", "Các loại Complex", "Сложные типы", "complex Types"));
			Rows.Add( new LocalizationRow("ID_LABEL_COMPLEX_ARRAYS", "Complex Arrays", "Les tableaux complexes", "Array complesse", "Komplexe Arrays", "Las matrices complejas", "المصفوفات المعقدة", "复杂的阵列", "複雑な配列", "복잡한 배열", "Mảng Complex", "Сложные Массивы", "complex Arrays"));
			Rows.Add( new LocalizationRow("ID_LABEL_CREATION_OPTIONS", "Creation Options", "Options de création", "Opzioni di creazione", "Erstellungsoptionen", "Opciones de creación", "خيارات خلق", "创建选项", "作成オプション", "작성 옵션", "Tùy chọn Creation", "Опции создания", "Creation Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PLAYMAKER", "Generate Playmaker Actions", "Générer Playmaker Actions", "Genera Playmaker azioni", "Generieren Spielmacher Aktionen", "Generar acciones Playmaker", "توليد تطبيقات صانع ألعاب", "生成策动进攻行动", "プレーメーカーのアクションを生成します", "플레이 메이커 작업을 생성", "Tạo Playmaker Actions", "Сформировать плеймейкер действия", "Genereer Playmaker Acties"));
			Rows.Add( new LocalizationRow("ID_LABEL_PERSIST_SCENE_LOADING", "Persist Scene Loading", "Persister Scène Chargement", "Persistono Scene Caricamento", "Persist Szene Loading", "Cargando persistir Escena", "تستمر المشهد تحميل", "坚持场景加载", "シーンの読み込みを永続化", "장면로드를 지속", "Cố Cảnh tải", "Упорство Сцена Загрузка", "Persist Scene Loading"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_FORMATTING", "JSON Formatting Options", "JSON options de formatage", "Opzioni di formattazione JSON", "JSON Formatierungsoptionen", "Opciones de formato JSON", "خيارات تنسيق JSON", "JSON格式选项", "JSON書式オプション", "JSON 포맷 옵션", "JSON Tùy chọn định dạng", "Параметры форматирования JSON", "JSON opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_EXPORT_CLASS", "Export Generated Class", "Classe Export Généré", "Export classe generata", "Export generierte Klassen", "Clase generada de exportación", "تصدير الفئة التي تم إنشاؤها", "导出生成的类", "エクスポート生成されたクラス", "수출 생성 된 클래스", "Xuất khẩu được tạo ra lớp", "Экспорт созданного класса", "Export Generated Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_EXPORT_TYPE", "JSON Export Type", "JSON Type d'exportation", "JSON tipo di esportazione", "JSON Exportart", "JSON Tipo de exportación", "JSON تصدير نوع", "JSON导出类型", "JSONエクスポートタイプ", "JSON 내보내기 유형", "JSON Loại khẩu", "Экспорт в формате JSON Тип", "JSON Export Type"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_UNICODE", "Escape Unicode Strings", "Évadez Unicode Strings", "Fuga Unicode stringhe", "Flucht Unicode-Strings", "Cuerdas de escape Unicode", "هروب يونيكود سلاسل", "逃避unicode字符串", "Unicodeの文字列をエスケープ", "유니 코드 문자열을 탈출", "Thoát Unicode Strings", "Побег Unicode строк", "Escape Unicode Strings"));
			Rows.Add( new LocalizationRow("ID_LABEL_CONVERT_CELL_ARRAYS", "Convert Cell Array to String", "Convertir Mobile Array Chaîne", "Convertire cellulare Array di String", "Konvertieren Zellenfeld String", "Convertir serie de células de cuerdas", "تحويل صفيف خلية لسلسلة", "转换单元阵列为String", "セル配列は、文字列に変換します", "String으로 셀 어레이로 변환", "Chuyển đổi tế bào mảng String", "Преобразование элемент массива в строку", "Omzetten Cell Array naar String"));
			Rows.Add( new LocalizationRow("ID_LABEL_VALIDATE_WORKSHEET", "Validate Worksheet", "Valider la feuille de travail", "Convalida del foglio di lavoro", "Validate-Arbeitsblatt", "Hoja de trabajo Validar", "التحقق من صحة ورقة عمل", "验证工作表", "検証ワークシート", "검증 워크 시트", "Validate Worksheet", "Подтвердить Рабочий лист", "Bevestigen werkblad"));
			Rows.Add( new LocalizationRow("ID_LABEL_VALIDATE_WORKBOOK", "Validate All Worksheets", "Valider toutes les feuilles", "Convalida tutti i fogli", "Bestätigen Sie alle Arbeitsblätter", "Validar todas las hojas", "التحقق من صحة جميع أوراق العمل", "验证所有工作表", "すべてのワークシートを検証", "모든 워크 시트의 유효성을 검사합니다", "Xác nhận Tất cả Worksheets", "Подтвердить все листы", "Valideren alle werkbladen"));
			Rows.Add( new LocalizationRow("ID_LABEL_CSV_FORMATTING", "CSV Formatting Options", "CSV Options de formatage", "Opzioni di formattazione CSV", "CSV Formatierungsoptionen", "Opciones de formato CSV", "خيارات تنسيق CSV", "CSV格式选项", "CSV書式オプション", "CSV 포맷 옵션", "Tùy chọn định dạng CSV", "CSV Параметры форматирования", "CSV opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_NGUI_FORMATTING", "NGUI Formatting Options", "Ngui options de formatage", "Opzioni di formattazione Ngui", "Ngui Formatierungsoptionen", "Opciones de formato ngui", "خيارات تنسيق NGUI", "NGUI格式选项", "NGUI書式オプション", "NGUI 서식 옵션", "Ngừi Tùy chọn định dạng", "NGUI Параметры форматирования", "Ngui opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_STRINGS", "Convert Quotes", "convertir Quotes", "Convertire Citazioni", "Rechnen Sie Zitate", "Convertir Cotizaciones", "تحويل أسعار", "转换行情", "引用符を変換します", "지수 변환", "chuyển đổi Quotes", "Преобразование Котировки", "omzetten Quotes"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_OBJECT_PREVIEW", "Preview JSON Object", "Aperçu JSON Object", "Anteprima JSON oggetto", "Vorschau JSON-Objekt", "Objeto de previsualización JSON", "كائن معاينة JSON", "预览JSON对象", "プレビューJSONオブジェクト", "미리보기 JSON 개체", "Xem trước JSON Object", "Просмотр JSON объекта", "Voorbeschouwing JSON Object"));
			Rows.Add( new LocalizationRow("ID_LABEL_JSON_CLASS_PREVIEW", "Preview JSON Class", "Aperçu JSON Class", "Anteprima JSON Class", "Vorschau JSON-Klasse", "Vista previa de la clase JSON", "معاينة JSON الفئة", "预览JSON类", "プレビューJSONクラス", "미리보기 JSON 클래스", "Xem trước JSON Lớp", "Предварительный просмотр в формате JSON Класс", "Voorbeschouwing JSON Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_PREVIEW", "Generate Preview", "Générer Aperçu", "genera anteprima", "generieren Vorschau", "generar Vista previa", "توليد معاينة", "生成预览", "プレビューを生成します", "미리보기를 생성", "tạo Preview", "Сформировать Preview", "genereren Voorbeeld"));
			Rows.Add( new LocalizationRow("ID_LABEL_GENERATE_CLASS", "Generate Class", "Générer la classe", "generare Classe", "generieren Klasse", "generar Clase", "توليد الدرجة", "生成类", "クラスを生成します", "클래스를 생성", "tạo lớp", "Генерация класса", "Genereer Class"));
			Rows.Add( new LocalizationRow("ID_LABEL_CSV_PREVIEW", "Preview CSV File", "Aperçu fichier CSV", "Anteprima file CSV", "Vorschau CSV Datei", "Previsualización de archivos CSV", "معاينة ملف CSV", "预览CSV文件", "プレビューCSVファイル", "미리보기 CSV 파일", "Xem trước tập tin CSV", "Предварительный просмотр файла CSV", "Voorbeschouwing CSV-bestand"));
			Rows.Add( new LocalizationRow("ID_LABEL_NGUI_PREVIEW", "Preview NGUI File", "Aperçu Ngui Fichier", "File Anteprima Ngui", "Vorschau Ngui Datei", "Archivo de previsualización ngui", "ملف المعاينة NGUI", "预览NGUI文件", "プレビューNGUIファイル", "미리보기 NGUI 파일", "Xem trước ngừi file", "Предварительный просмотр файла NGUI", "Voorbeschouwing Ngui File"));
			Rows.Add( new LocalizationRow("ID_LABEL_XML_PREVIEW", "Preview XML File", "Aperçu de fichier XML", "Anteprima file XML", "Vorschau XML Datei", "Previsualización de archivos XML", "معاينة ملف XML", "预览XML文件", "プレビューXMLファイル", "미리 XML 파일", "Xem trước tập tin XML", "Просмотр XML-файла", "Voorbeschouwing XML File"));
			Rows.Add( new LocalizationRow("ID_LABEL_XML_FORMATTING", "XML Formatting Options", "Options de formatage XML", "Opzioni di formattazione XML", "XML-Formatierungsoptionen", "Opciones de formato XML", "خيارات تنسيق XML", "XML格式化选项", "XML書式オプション", "XML 포맷 옵션", "Tùy chọn định dạng XML", "Параметры XML Форматирование", "XML opmaak Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_SHOWONSTARTUP", "Show On Startup", "Show On Startup", "Mostra Su di avvio", "Show On Startup", "Mostrar en el inicio", "تظهر عند بدء التشغيل", "在启动时显示", "起動時に表示", "시작에서보기", "Hiện On Startup", "Показать при запуске", "Show On Startup"));
			Rows.Add( new LocalizationRow("ID_LABEL_CULL_COLUMNS", "Stop On Blank Column", "Arrêt sur colonne Blank", "STOP sulla colonna vuota", "Stoppen Sie auf leere Säule", "Deténgase en una columna en blanco", "وقف على عمود فارغ", "停在空白列", "空欄で停止", "빈 열에서 중지", "Dừng Mở Cột Trống", "Остановка на пустой колонке", "Stop op lege kolom"));
			Rows.Add( new LocalizationRow("ID_LABEL_CULL_ROWS", "Stop On Blank Row", "Arrêt sur ligne vide", "STOP sulla riga vuota", "Stoppen Sie auf leere Zeile", "Deténgase en una fila en blanco", "وقف في صف فارغ", "停在空白行", "空白行で停止", "빈 행에 중지", "Dừng Mở Row Trống", "Остановка на пустую строку", "Stop op lege rij"));
			Rows.Add( new LocalizationRow("ID_LABEL_ESCAPE_LINE_BREAKS", "Escape Line Breaks", "Évadez-Line Breaks", "Fuga interruzioni di riga", "Entfliehen Sie Zeilenumbrüche", "Escapar saltos de línea", "الهروب خط فواصل", "逃脱换行符", "改行をエスケープ", "줄 바꿈을 탈출", "Thoát khỏi dòng Breaks", "Побег разрывы строк", "Escape Line Breaks"));
			Rows.Add( new LocalizationRow("ID_LABEL_LEGACY_OPTIONS", "Legacy Options", "Options héritées", "Opzioni impostate in precedenza", "Legacy-Optionen", "Opciones adicionales", "خيارات تراث", "传统选项", "レガシーオプション", "레거시 옵션", "Tùy chọn Legacy", "Дополнительные опции", "Legacy-opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_LOWERCASE_HEADER", "Make Columns Lowercase", "Faire Colonnes Minuscules", "Rendere le colonne Minuscolo", "Machen Sie Spalten Klein", "Hacer columnas en minúsculas", "جعل الأعمدة حرف صغير", "让列小写", "列の小文字を作ります", "열 소문자를 확인", "Hãy Cột Chữ thường", "Сделать колонны Строчные", "Maak Columns kleine letters"));
			Rows.Add( new LocalizationRow("ID_LABEL_TYPEROWBOX_HEADER", "Change Row 2 To Type Row?", "Change Row 2 au type Row?", "Change Row 2 per tipo di riga?", "Ändern Row 2 Um Zeilentyp?", "Cambio Fila 2 Fila Para escribir?", "تغيير صف 2 لاكتب الصف؟", "更改行2至类型行？", "変更行2は、行を入力するには？", "변경 행 2 행을 입력하려면?", "Thay đổi Row 2 Để Gõ Row?", "Изменить строку 2 Для типа строки?", "Change Row 2 To Row Type?"));
			Rows.Add( new LocalizationRow("ID_LABEL_TYPEROWBOX_MESSAGE", "The second row will be converted to a Type row, which will change the entries in your Google Spreadsheet. Are you sure you want to do this?", "La deuxième ligne sera convertie en une ligne Type, qui va changer les entrées dans votre feuille de calcul Google. Es-tu sûr de vouloir faire ça?", "La seconda fila sarà convertito in una fila tipo, che cambierà le voci nel vostro foglio di calcolo di Google. Sei sicuro di volerlo fare?", "Die zweite Reihe wird zu einer Art Reihe umgewandelt werden, die die Einträge in Ihrem Google-Tabelle ändern wird. Sind Sie sicher, dass Sie dies tun?", "La segunda fila se convertirá en una fila Tipo, que cambiará las entradas en su hoja de cálculo de Google. ¿Seguro que quieres hacer esto?", "سيتم تحويل الصف الثاني إلى الصف النوع، والتي سوف تغير إدخالات في جدول جوجل الخاص بك. هل أنت متأكد أنك تريد أن تفعل هذا؟", "第二行会被转换为连续型，这将改变你的谷歌电子表格中的条目。你确定要这么做吗？", "2行目は、Googleスプレッドシートのエントリが変更される、タイプの列に変換されます。あなたがこれを行うにしてもよろしいですか？", "두 번째 행이 Google 스프레드 시트의 항목을 변경하는 유형의 행으로 변환됩니다. 이 작업을 수행 하시겠습니까?", "Hàng thứ hai sẽ được chuyển đổi sang một dòng Type, mà sẽ thay đổi các mục trong bảng tính Google của bạn. Bạn có chắc chắn muốn làm điều này?", "Второй ряд будет преобразован в тип строки, которая будет изменять записи в электронной таблице Google. Вы уверены, что хотите это сделать?", "De tweede rij zal worden omgezet naar een Type rij, waarmee de items in uw Google-spreadsheet zal veranderen. Weet je zeker dat je dit wilt doen?"));
			Rows.Add( new LocalizationRow("ID_LABEL_CODE_GENERATION_OPTIONS", "Code Generation Options", "Options de génération de code", "Opzioni generazione del codice", "Code-Generierung Optionen", "Opciones de generación de código", "خيارات الجيل كود", "代码生成选项", "コー​​ド生成オプション", "코드 생성 옵션", "Tùy chọn hệ Mã", "Параметры создания кода", "Code Generation Opties"));
			Rows.Add( new LocalizationRow("ID_LABEL_PREPEND_UNDERSCORES", "Prepend Underscores to Variable Names", "Prepend soulignement noms de variables", "Anteporre sottolinea ai nomi di variabile", "Prepend Unterstreicht zu Variablennamen", "Anteponer Destaca en nombres de variables", "إضافة قبل يشدد على أسماء متغير", "前面加上下划线变量名", "先頭に追加は、変数名に下線", "앞에 추가는 변수 이름에 밑줄", "Thêm vào trước dấu gạch dưới để tên biến", "Prepend Подчеркивает для имен переменных", "Prepend onderstreept om namen van variabelen"));
			Rows.Add( new LocalizationRow("ID_LABEL_XML_COLUMN_AS_CHILD_TAGS", "Use Type Row", "Type d'utilisation Row", "Usa Tipo riga", "Verwenden Typ Row", "Uso Tipo de fila", "استخدام نوع الصف", "使用类型行", "タイプ行を使用します", "사용 유형 행", "Sử dụng Loại Row", "Использование Тип Row", "Gebruik Type Row"));
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
