# README #

![srbugreport](https://user-images.githubusercontent.com/17733911/41142146-9a224182-6b2f-11e8-89a4-c6fb23bb2b80.gif)  
### TextFile Mode
![srbugreport_textfile](https://user-images.githubusercontent.com/17733911/41142932-787f4a90-6b32-11e8-855b-ad2a0f882849.gif)
### Plane Mode  
![srbugreport_plane](https://user-images.githubusercontent.com/17733911/41143658-06ac4a32-6b35-11e8-9922-6d21007e87e2.gif)


# License  
MIT


# 使い方
1. SRDebuggerをインポート済みのプロジェクトにこのプロジェクトを追加して下さい。
1.  https://api.slack.com/custom-integrations/legacy-tokens でAPIトークンを取得してください。
1. `SRBugReportSendToSlack/Settings.cs`にAPIトークンなどを入力して下さい。  
1. `StompyRobot/SRDebugger/Scripts/Services/Implementation/BugReportApiService.cs`内のBugReportApiをMyBugReportApiに変更して下さい。

### Plane Modeで使用する場合  
1.https://bugreporttest.slack.com/apps/A0F7XDUAZ-incoming-webhook でWebHook URLを取得して下さい。  
1.`SRBugReportSendToSlack/Settings.cs`のWebHookUrlに取得したURLを設定して下さい。  

# Release Page
https://github.com/Itoen/Unity-SRDebugger_BugReport_Send_To_Slack/releases/
