# README #

![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172322.png)
![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172421.png)
![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172426.png)


# 使い方
1. SRDebuggerをインポート済みのプロジェクトにこのプロジェクトを追加して下さい。
1.  https://api.slack.com/custom-integrations/legacy-tokens でAPIトークンを取得してください。
1. `SRBugReportSendToSlack/SlackAPISetting.cs`にAPIトークンなどを入力して下さい。  
1. `StompyRobot/SRDebugger/Scripts/Services/Implementation/BugReportApiService.cs`内のBugReportApiをMyBugReportApiに変更して下さい。
