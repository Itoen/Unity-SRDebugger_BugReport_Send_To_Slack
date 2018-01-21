# README #

![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172322.png)
![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172421.png)
![Alt text](https://cdn-ak.f.st-hatena.com/images/fotolife/t/tc_kazuki/20180121/20180121172426.png)


# 使い方
1. SRDebuggerをインポート済みのプロジェクトにこのプロジェクトを追加して下さい。
2. `StompyRobot/SRDebugger/Scripts/Services/Implementation/BugReportApiService.cs`を編集して下さい。
> private BugReportApi _reportApi;
> ↓
> private MyBugReportApi _reportApi;

> _reportApi = new BugReportApi(report, Settings.Instance.ApiKey);
> ↓
> _reportApi = new MyBugReportApi(report, Settings.Instance.ApiKey);
