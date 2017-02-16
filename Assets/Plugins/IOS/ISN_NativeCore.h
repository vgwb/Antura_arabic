////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


@interface NSData (Base64)

+ (NSData *)InitFromBase64String:(NSString *)aString;
- (NSString *)AsBase64String;

@end

@interface NSDictionary (JSON)

- (NSString *)AsJSONString;

@end

@interface ISN_DataConvertor : NSObject

+ (NSString*) charToNSString: (char*)value;
+ (const char *) NSIntToChar: (NSInteger) value;
+ (const char *) NSStringToChar: (NSString *) value;
+ (NSArray*) charToNSArray: (char*)value;

+ (const char *) serializeErrorWithData:(NSString *)description code: (int) code;
+ (const char *) serializeError:(NSError *)error;

+ (NSMutableString *) serializeErrorWithDataToNSString:(NSString *)description code: (int) code;
+ (NSMutableString *) serializeErrorToNSString:(NSError *)error;


+ (const char *) NSStringsArrayToChar:(NSArray *) array;
+ (NSString *) serializeNSStringsArray:(NSArray *) array;

@end


@interface ISN_NativeUtility : NSObject

@property (strong)  UIActivityIndicatorView *spinner;

+ (id) sharedInstance;
+ (int) majorIOSVersion;
+ (BOOL) IsIPad;

- (void) redirectToRatingPage: (NSString *) appId;
- (void) setApplicationBagesNumber:(int) count;

- (void) ShowSpinner;
- (void) HideSpinner;
- (void) ISN_NativeLog: (NSString *) appId, ...;
- (void) ISN_SetLogState: (BOOL) appId;

@end

@interface CloudManager : NSObject


+ (id) sharedInstance;

- (void) initialize;
- (void) setString:(NSString*) val key:(NSString*) key;
- (void) setDouble:(double) val key:(NSString*) key;
- (void) setData:(NSData*) val key:(NSString*) key;

-(void) requestDataForKey:(NSString*) key;

@end







@interface ISNSharedApplication : NSObject

+ (id)  sharedInstance;

- (void) checkUrl:(NSString*)url;
- (void) openUrl:(NSString*)url;


@end






@interface ISN_NativePopUpsManager : NSObject
+ (ISN_NativePopUpsManager *) sharedInstance;
@end



@interface IOSNativeNotificationCenter : NSObject


+ (IOSNativeNotificationCenter *)sharedInstance;
- (void) scheduleNotification: (int) time message: (NSString*) message sound: (bool *)sound alarmID:(NSString *)alarmID badges: (int)badges notificationData: (NSString*) notificationData notificationSoundName: (NSString*) notificationSoundName;
- (void) cleanUpLocalNotificationWithAlarmID: (NSString *)alarmID;
- (void) cancelNotifications;
- (void) applicationIconBadgeNumber: (int)badges;
- (void) RegisterForNotifications;

@end










