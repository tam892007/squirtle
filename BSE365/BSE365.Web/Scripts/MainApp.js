var mainApp=angular.module("mainApp",["ui.router","ui.bootstrap","authApp","ngResource","ui.tree","smart-table","underscore","angularFileUpload","ngImgCrop","ngMessages","angular-loading-bar","ui.validate","reCAPTCHA","commonApp","ui-notification","timer"]);mainApp.config(["$stateProvider","$urlRouterProvider",function(e,t){t.otherwise("/login"),e.state("home",{url:"/",templateUrl:"app/main/home/home.html"}).state("login",{url:"/login",templateUrl:"app/authentication/login/login.html",controller:"loginController"}).state("resetPassword",{url:"/resetpassword",templateUrl:"app/authentication/password/resetPassword.html",controller:"resetPasswordController"}).state("refresh",{url:"/refresh",templateUrl:"app/authentication/refresh/refresh.html",controller:"refreshController"}).state("pin",{"abstract":!0,templateUrl:"app/common/templates/empty.html"}).state("pin.transfer",{url:"/transfer",templateUrl:"app/main/pin/pin.html",controller:"pinController"}).state("pin.rate",{url:"/rate",templateUrl:"app/main/pin/rate.html",controller:"rateController"}).state("user",{"abstract":!0,url:"/user",templateUrl:"app/main/user/user-info.html",controller:"userController"}).state("user.default",{url:"/",templateUrl:"app/main/user/user-info.default.html",controller:"userDefaultController"}).state("user.register",{url:"/register",templateUrl:"app/main/user/user-info.register.html",controller:"userRegisterController"}).state("user.tree",{url:"/tree",templateUrl:"app/main/user/user-info.tree.html",controller:"userTreeController"}).state("trade",{"abstract":!0,url:"/trade",templateUrl:"app/main/trade/trade-info.html",controller:"tradeInfoController"}).state("trade.statistic",{url:"/",templateUrl:"app/main/trade/trade-statistic.html",controller:"tradeStatisticController"}).state("trade.history",{url:"/history",templateUrl:"app/main/trade/trade-history.html",controller:"tradeHistoryController"}).state("waitinggiver",{url:"/waitinggiver",templateUrl:"app/main/account/waitingGiver.html",controller:"waitingGiverController"}).state("waitingreceiver",{url:"/waitingreceiver",templateUrl:"app/main/account/waitingReceiver.html",controller:"waitingReceiverController"}).state("account",{"abstract":!0,url:"/account",templateUrl:"app/main/account/account.html",controller:"accountController"}).state("account.default",{url:"/",templateUrl:"app/main/account/account-default.html",controller:function(){}}).state("account.details",{url:"/:key",templateUrl:"app/main/account/account-edit.html",controller:"accountInfoController"}).state("currentTransaction",{url:"/trade/current",templateUrl:"app/main/transaction/current.html",controller:"transactionCurrentController"}).state("association",{url:"/association",templateUrl:"app/main/association/association.html",controller:"associationController"})}]),mainApp.config(["$httpProvider",function(e){e.interceptors.push("authInterceptorService")}]),mainApp.run(["authService",function(e){e.fillAuthData()}]),mainApp.config(["treeConfig",function(e){e.defaultCollapsed=!0}]),mainApp.config(["cfpLoadingBarProvider",function(e){e.includeSpinner=!1}]),mainApp.config(["reCAPTCHAProvider","recaptchaSettings",function(e,t){e.setPublicKey(t.publicKey),e.setOptions({theme:"red"})}]),mainApp.config(["NotificationProvider",function(e){e.setOptions({delay:1500,startTop:20,startRight:10,verticalSpacing:20,horizontalSpacing:20,positionX:"right",positionY:"bottom",closeOnClick:!0,maxCount:3})}]),mainApp.factory("AccountState",function(){var e={Default:0,WaitGive:1,Gave:2,WaitReceive:3,InGiveTransaction:11,InReceiveTransaction:12,NotGive:21,NotConfirm:22,ReportedNotTransfer:23,AbadonedOne:31};return e.display=function(t){switch(t){case e.Default:return"Default";case e.WaitGive:return"Wait Give";case e.Gave:return"Gave";case e.WaitReceive:return"Wait Receive";case e.InGiveTransaction:return"In Give Transaction";case e.InReceiveTransaction:return"In Receive Transaction";case e.NotGive:return"Not Give";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";case e.AbadonedOne:return"Abadoned One";default:return""}},e}),mainApp.factory("PriorityLevel",function(){var e={Default:0,Priority:1,High:2,ReceiveFail:3,Highest:10};return e.display=function(t){switch(t){case e.Default:return"Default";case e.Priority:return"Priority";case e.High:return"High";case e.ReceiveFail:return"Receive Fail";case e.Highest:return"Highest";default:return""}},e}),mainApp.factory("TransactionState",function(){var e={Begin:0,Transfered:1,Confirmed:2,NotTransfer:21,NotConfirm:22,ReportedNotTransfer:23,Abadoned:31};return e.display=function(t){switch(t){case e.Begin:return"Begin";case e.Transfered:return"Transfered";case e.Confirmed:return"Confirmed";case e.NotTransfer:return"Not Transfer";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";case e.Abadoned:return"Abadoned";default:return""}},e}),mainApp.factory("TransactionType",function(){var e={Begin:0,Abadoned:31,Replacement:41};return e.display=function(t){switch(t){case e.Begin:return"Begin";case e.Abadoned:return"Abadoned";case e.Replacement:return"Replacement";default:return""}},e}),mainApp.factory("UserState",function(){var e={Default:0,NotGive:21,NotConfirm:22};return e.display=function(t){switch(t){case e.Default:return"Default";case e.NotGive:return"Not Give";case e.NotConfirm:return"Not Confirm";default:return""}},e}),mainApp.factory("ConfigData",function(){var e={dateFormat:"yyyy/MM/dd",dateTimeFormat:"yyyy/MM/dd"};return e}),mainApp.filter("yesno",[function(){return function(e){return e?"Yes":"No"}}]),mainApp.controller("accountController",["$scope","$state","accountService","Notification","AccountState","PriorityLevel","UserState","ConfigData",function(e,t,r,a,i,n,o,s){e.loadData=function(){e.data=[],r.queryAccount({},function(t){e.data=t})},e.viewDefails=function(r){e.current&&(e.current.selected=!1),r.selected=!0,e.current=r,t.go("^.details",{key:r.userName})},e.getAccountState=function(e){var t=e.UserState==o.NotGive||e.UserState==o.NotConfirm?o.display(e.userState)+" ("+e.relatedAccount+")":i.display(e.state);return t},e.init=function(){e.AccountState=i,e.PriorityLevel=n,e.UserState=o,e.ConfigData=s,e.data=[],e.loadData()},e.init()}]),mainApp.controller("accountInfoController",["$scope","$stateParams","accountService","Notification","AccountState","PriorityLevel","UserState",function(e,t,r,a,i,n,o){e.getData=function(){r.status({key:t.key},{key:t.key},function(t){e.info=t,e.info.newPriority=e.info.priority,e.info.newState=e.info.state})},e.queueGive=function(){e.info.isAllowGive=!1,r.queueGive({},function(t){a.success("Queue give successful!"),e.updateStatus()},function(t){e.info.isAllowGive=!0,a.success(t)})},e.queueReceive=function(){e.info.isAllowReceive=!1,r.queueReceive({},function(t){a.success("Queue receive successful!"),e.updateStatus()},function(t){e.info.isAllowReceive=!0,a.success(t)})},e.setPriority=function(){r.setAccountPriority({userName:e.info.userName,priority:e.info.newPriority},function(e){a.success("Priority Changed!")})},e.setAccountState=function(){r.setAccountState({userName:e.info.userName,state:e.info.newState},function(t){a.success("State Changed!"),e.getData()})},e.init=function(){e.UserState=o,e.AccountState=i,e.accountStateToSelect=[{value:i.Default,text:i.display(i.Default)},{value:i.Gave,text:i.display(i.Gave)}],e.PriorityLevel=n,e.priorityToSelect=[{value:n.Default,text:n.display(n.Default)},{value:n.Priority,text:n.display(n.Priority)},{value:n.High,text:n.display(n.High)},{value:n.Highest,text:n.display(n.Highest)}],e.getData()},e.init()}]),mainApp.factory("accountService",["$resource",function(e){return e(":path",{},{status:{method:"POST",params:{path:"api/trade/accountStatus",key:"key"}},setAccountPriority:{method:"POST",params:{path:"api/trade/setAccountPriority"}},setAccountState:{method:"POST",params:{path:"api/trade/setAccountState"}},queueGive:{method:"POST",params:{path:"api/trade/queueGive",key:"key"}},queueReceive:{method:"POST",params:{path:"api/trade/queueReceive",key:"key"}},queryAccount:{method:"POST",params:{path:"api/trade/queryAccount"},isArray:!0},queryHistory:{method:"POST",params:{path:"api/trade/queryHistory",key:"key"},isArray:!0},queryWaitingGivers:{method:"POST",params:{path:"api/trade/queryWaitingGivers"},isArray:!0},queryWaitingReceivers:{method:"POST",params:{path:"api/trade/queryWaitingReceivers"},isArray:!0}})}]),mainApp.controller("waitingGiverController",["$scope","$state","accountService","Notification","PriorityLevel","ConfigData",function(e,t,r,a,i,n){e.loadData=function(){e.data=[],r.queryWaitingGivers({},function(t){e.data=t})},e.viewDefails=function(t){e.target&&(e.target.selected=!1),t.selected=!0,e.target=t,e.selected=!0},e.init=function(){e.PriorityLevel=i,e.ConfigData=n,e.data=[],e.target={},e.selected=!1,e.loadData()},e.init()}]),mainApp.controller("waitingReceiverController",["$scope","$state","accountService","Notification","PriorityLevel","ConfigData",function(e,t,r,a,i,n){e.loadData=function(){e.data=[],r.queryWaitingReceivers({},function(t){e.data=t})},e.viewDefails=function(t){e.target&&(e.target.selected=!1),t.selected=!0,e.target=t,e.selected=!0},e.init=function(){e.PriorityLevel=i,e.ConfigData=n,e.data=[],e.target={},e.selected=!1,e.loadData()},e.init()}]),mainApp.controller("associationController",["$scope","userService","authService","$state",function(e,t,r,a){e.init=function(){e.associatedUsrs=[],e.selectedUsr={}},e.init(),e.getAssociation=function(){t.getCurrentAssociation(function(t){e.associatedUsrs=t})},e.getAssociation(),e["switch"]=function(t){e.selectedUsr.id!=t.id&&(e.password="",e.selectedUsr=t)},e.login=function(){var t={userName:e.selectedUsr.userName,password:e.selectedUsr.password,useRefreshTokens:!1};r.login(t).then(function(e){a.go("home")},function(t){e.message="Wrong password!"})},e.isMain=function(t){return null==t||t.userName.endsWith("A")||1==e.associatedUsrs.length}}]),mainApp.controller("indexController",["$scope","$state","authService","userService","$location",function(e,t,r,a,i){e.logOut=function(){r.logOut(),t.go("login")},e.authentication=r.authentication,e.$on("user:authenticated",function(t,r){e.getUserContext()}),e.$on("user:updateAvatar",function(t,r){e.userContext.avatar.url=r}),e.getUserContext=function(){a.getCurrentUserContext(function(t){e.userContext=t})};var n=i.search();e.authentication.isAuth&&"true"!=n.anonymous&&e.getUserContext()}]),mainApp.controller("pinController",["$scope","userService","pinService","$q","Notification","$state","$window",function(e,t,r,a,i,n,o){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={step:1}},e.init(),e.processToConfirm=function(){e.submitted=!0,e.frmTransfer.$valid&&(e.transaction.step=2)},e.goBack=function(){e.transaction.step=1},e.transferPIN=function(){r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),n.reload()},function(t){i.error("Some errors happenned"),e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2,o.Recaptcha.reload())})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(r){var i=a.defer();return t.checkName({name:r},function(t){t.isSuccessful?(e.toUser=t.result,i.resolve(t)):(e.toUser={},i.reject(t))}),i.promise}}]),mainApp.factory("pinService",["$resource",function(e){return e(":path",{},{transfer:{method:"POST",params:{path:"api/pin/transfer",transactionVM:"transactionVM"}},getCurrentUserHistory:{method:"GET",params:{path:"api/pin/getAll"},isArray:!0}})}]),mainApp.controller("rateController",["$scope","$q","_","$http",function(e,t,r,a){e.rate={},e.bitPrices=[],e.init=function(){e.bitPrices[0]={},e.bitPrices[1]={},e.bitPrices[2]={},e.bitPrices[3]={},e.bitPrices[4]={},e.bitPrices[5]={},e.bitPrices[6]={},e.bitPrices[0].quantity1=1,e.bitPrices[0].quantity2=50,e.bitPrices[1].quantity1=51,e.bitPrices[1].quantity2=100,e.bitPrices[2].quantity1=101,e.bitPrices[2].quantity2=200,e.bitPrices[3].quantity1=201,e.bitPrices[3].quantity2=300,e.bitPrices[4].quantity1=301,e.bitPrices[4].quantity2=400,e.bitPrices[5].quantity1=401,e.bitPrices[5].quantity2=500,e.bitPrices[6].quantity1=501,e.bitPrices[6].quantity2=1e3,e.bitPrices[0].usd=9.09,e.bitPrices[1].usd=8.18,e.bitPrices[2].usd=7.27,e.bitPrices[3].usd=6.36,e.bitPrices[4].usd=5.45,e.bitPrices[5].usd=4.55,e.bitPrices[6].usd=3.64,a({method:"GET",url:"http://api.coindesk.com/v1/bpi/currentprice.json",skipInterceptor:!0}).then(function(t){e.rate.usd=t.data.bpi.USD.rate_float;for(var r=0;r<e.bitPrices.length;r++)e.bitPrices[r].bitCoin=e.bitPrices[r].usd/e.rate.usd,e.bitPrices[r].totalBitCoin=e.bitPrices[r].quantity2*e.bitPrices[r].bitCoin,e.bitPrices[r].totalUsd=e.bitPrices[r].usd*e.bitPrices[r].quantity2})},e.init()}]),mainApp.controller("tradeHistoryController",["$scope","$uibModal","tradeService","Notification","AccountState","ConfigData",function(e,t,r,a,i,n){e.loadData=function(){e.data=[],r.queryHistory({},function(t){e.data=t})},e.viewDefails=function(e){var r=t.open({templateUrl:"app/main/transaction/transactionHistory.html",size:"lg",controller:"transactionHistoryController",resolve:{targetData:function(){return e}}});r.result.then(function(e){},function(){})},e.init=function(){e.AccountState=i,e.ConfigData=n,e.data=[],e.loadData()},e.init()}]),mainApp.controller("tradeInfoController",["$scope","tradeService","Notification","AccountState","PriorityLevel","UserState",function(e,t,r,a,i,n){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){e.info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(t){e.info.isAllowGive=!0,r.success(t)})},e.queueReceive=function(){e.info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(t){e.info.isAllowReceive=!0,r.success(t)})},e.init=function(){e.AccountState=a,e.UserState=n,e.PriorityLevel=i,e.updateStatus()},e.init()}]),mainApp.factory("tradeService",["$resource",function(e){return e(":path",{},{status:{method:"POST",params:{path:"api/trade/accountStatus"}},queueGive:{method:"POST",params:{path:"api/trade/queueGive"}},queueReceive:{method:"POST",params:{path:"api/trade/queueReceive"}},queryHistory:{method:"POST",params:{path:"api/trade/queryHistory"},isArray:!0}})}]),mainApp.controller("tradeStatisticController",["$scope","tradeService","Notification","AccountState","PriorityLevel",function(e,t,r,a,i){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(e){info.isAllowGive=!0,r.success(e)})},e.queueReceive=function(){info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(e){info.isAllowReceive=!0,r.success(e)})},e.init=function(){e.AccountState=a,e.PriorityLevel=i,e.updateStatus()},e.init()}]),mainApp.controller("transactionCurrentController",["$scope","_","$timeout","transactionService","tradeService","Notification","AccountState","TransactionState","ConfigData",function(e,t,r,a,i,n,o,s,c){function u(r){var a=-1;switch(t.each(r,function(r){r.isBegin=r.state==s.Begin,r.isAllowConfirmGave=r.state==s.Begin,r.isAllowConfirmReceived=r.state==s.Transfered,0>a?r.state!=s.Abadoned&&(a=r.state):r.state==s.Abadoned||(a<s.NotTransfer?a=r.state>=s.NotTransfer?r.state:a>r.state?r.state:a:r.state>=s.NotTransfer&&(a=a>r.state?r.state:a)),console.log(a);var i="";"giving"==e.grState?(i=r.receiverId,r.receivedDate&&e.histories.push({userName:i,rating:r.rating,time:r.receivedDate,isCompleted:!0})):(i=r.giverId,r.transferedDate&&e.histories.push({userName:i,rating:r.rating,time:r.transferedDate,isCompleted:!0})),e.histories=t.sortBy(e.histories,function(e){return e.time})}),a){case s.Begin:e.overviewState.queued=1,e.overviewState.giving=0,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=-1;break;case s.Transfered:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=0,e.overviewState.ended=-1;break;case s.Confirmed:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=1,e.overviewState.ended=1;break;case s.NotTransfer:e.overviewState.queued=1,e.overviewState.giving=-1,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=1;break;case s.NotConfirm:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=-1,e.overviewState.ended=1;break;case s.ReportedNotTransfer:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=0}console.log(a),console.log(e.overviewState),e.transactions=r}e.updateStatus=function(){return i.status({},function(t){e.info=t}).$promise},e.getCurrentTransactions=function(){e.info.state==o.InGiveTransaction?(e.accountDisplayTemplate=e.receiverInfoTemplateUrl,e.grState="giving",a.giveRequested({},function(e){u(e)})):e.info.state==o.InReceiveTransaction&&(e.accountDisplayTemplate=e.giverInfoTemplateUrl,e.grState="receiving",a.receiveRequested({},function(e){u(e)}))},e.moneyTransfered=function(t){a.moneyTransfered(t,function(r){n.success("Money Transfered");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r)})},e.moneyReceived=function(t){a.moneyReceived(t,function(r){n.success("Money Received");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r)})},e.reportNotTransfer=function(t){a.reportNotTransfer(t,function(r){n.success("Transaction Reported");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r)})},e.init=function(){e.grState="",e.giverInfoTemplateUrl="app/main/transaction/info-giver.html",e.receiverInfoTemplateUrl="app/main/transaction/info-receiver.html",e.TransactionState=s,e.AccountState=o,e.ConfigData=c,e.overviewState={queued:1,giving:0,gave:-1,received:-1,ended:-1},e.histories=[],e.updateStatus().then(function(t){e.getCurrentTransactions()})},e.init()}]),mainApp.controller("transactionHistoryController",["$scope","_","$timeout","$uibModal","$uibModalInstance","transactionService","tradeService","Notification","ConfigData","AccountState","TransactionState","targetData",function(e,t,r,a,i,n,o,s,c,u,l,d){function p(r){var a=-1;switch(t.each(r,function(r){r.isBegin=r.state==l.Begin,r.isAllowConfirmGave=r.state==l.Begin,r.isAllowConfirmReceived=r.state==l.Transfered,0>a?r.state!=l.Abadoned&&(a=r.state):r.state==l.Abadoned||(a<l.NotTransfer?a=r.state>=l.NotTransfer?r.state:a>r.state?r.state:a:r.state>=l.NotTransfer&&(a=a>r.state?r.state:a)),console.log(a);var i="";"giving"==e.grState?(i=r.receiverId,r.receivedDate&&e.histories.add({userName:i,rating:r.rating,time:r.receivedDate,isCompleted:!0})):(i=r.giverId,r.transferedDate&&e.histories.add({userName:i,rating:r.rating,time:r.transferedDate,isCompleted:!0})),e.histories=t.sortBy(e.histories,function(e){return e.time})}),a){case l.Begin:e.overviewState.queued=1,e.overviewState.giving=0,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=-1;break;case l.Transfered:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=0,e.overviewState.ended=-1;break;case l.Confirmed:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=1,e.overviewState.ended=1;break;case l.NotTransfer:e.overviewState.queued=1,e.overviewState.giving=-1,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=1;break;case l.NotConfirm:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=1,e.overviewState.received=-1,e.overviewState.ended=1;break;case l.ReportedNotTransfer:e.overviewState.queued=1,e.overviewState.giving=1,e.overviewState.gave=-1,e.overviewState.received=-1,e.overviewState.ended=0}console.log(a),console.log(e.overviewState),e.transactions=r}e.getCurrentTransactions=function(){e.targetData.type==u.InGiveTransaction?(e.accountDisplayTemplate=e.receiverInfoTemplateUrl,e.grState="giving"):e.targetData.type==u.InReceiveTransaction&&(e.accountDisplayTemplate=e.giverInfoTemplateUrl,e.grState="receiving"),n.history(d,function(e){p(e)})},e.init=function(){e.targetData=d,e.grState="",e.giverInfoTemplateUrl="app/main/transaction/info-giver.html",e.receiverInfoTemplateUrl="app/main/transaction/info-receiver.html",e.TransactionState=l,e.AccountState=u,e.ConfigData=c,e.overviewState={queued:1,giving:0,gave:-1,received:-1,ended:-1},e.histories=[],e.getCurrentTransactions()},e.init()}]),mainApp.factory("transactionService",["$resource",function(e){return e(":path",{},{history:{method:"POST",params:{path:"api/transaction/history",key:"key"},isArray:!0},giveRequested:{method:"POST",params:{path:"api/transaction/giveRequested"},isArray:!0},receiveRequested:{method:"POST",params:{path:"api/transaction/receiveRequested"},isArray:!0},moneyTransfered:{method:"POST",params:{path:"api/transaction/moneyTransfered"}},moneyReceived:{method:"POST",params:{path:"api/transaction/moneyReceived"}},reportNotTransfer:{method:"POST",params:{path:"api/transaction/reportNotTransfer"}}})}]),mainApp.controller("dlgChangeAvatarController",["$scope","userService","FileUploader","_","localStorageService","$uibModalInstance","Notification",function(e,t,r,a,i,n,o){e.uploadImage=function(){var e=a.last(s.queue);e.upload()};var s=e.uploader=new r({autoUpload:!1,url:"api/user/updateAvatar"});s.filters.push({name:"size",fn:function(e){return e.size<=5242880}}),s.filters.push({name:"image",fn:function(e){return s.hasHTML5?/\/(png|jpeg|jpg)$/.test(e.file.type):!0}});var c=i.get("authorizationData");c&&(s.headers.Authorization="Bearer "+c.token),s.onAfterAddingFile=function(t){e.cropped={image:""};var r=new FileReader;r.onload=function(t){e.$apply(function(){e.image=t.target.result})},r.readAsDataURL(t._file)},s.onBeforeUploadItem=function(t){var r=u(e.cropped.image);t._file=r};var u=function(e){var t;t=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):decodeURI(e.split(",")[1]);for(var r=e.split(",")[0].split(":")[1].split(";")[0],a=[],i=0;i<t.length;i++)a.push(t.charCodeAt(i));return new Blob([new Uint8Array(a)],{type:r})};s.onSuccessItem=function(t,r,a,i){console.log(r),e.ok(r)},e.ok=function(e){o.success("Avatar has been updated"),n.close(e)},e.cancel=function(){n.dismiss()}}]),mainApp.controller("dlgChangePasswordController",["$scope","userService","$uibModalInstance","Notification",function(e,t,r,a){e.interacted=function(t){return e.submitted||t.$dirty},e.init=function(){e.failed=!1,e.submitted=!1,e.model={}},e.init(),e.changePassword=function(e){return t.changePassword(e).$promise},e.submit=function(){e.submitted=!0,e.frmPassword.$valid&&e.changePassword(e.model).then(function(t){t.result?e.ok():(e.failed=!0,a.error("Failed to update your password"))})},e.ok=function(e){a.success("Password has been changed successfully"),r.close(e)},e.cancel=function(){r.dismiss()}}]),mainApp.controller("userController",["$scope","userService","imageService","$uibModal",function(e,t,r,a){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.init=function(){e.currentUser=e.currentUser||null,null==e.currentUser&&e.getCurrentUserProfile().then(function(t){e.currentUser=t})},e.init(),e.updateAvatar=function(){var t=a.open({animation:!1,templateUrl:"app/main/user/dlg-change-avatar.html",controller:"dlgChangeAvatarController",size:"lg",windowClass:"portraitDialog"});t.result.then(function(t){e.currentUser=t,e.currentUser.avatar.url=e.currentUser.avatar.url+"?"+(new Date).getTime(),e.$emit("user:updateAvatar",e.currentUser.avatar.url)},function(){})}}]),mainApp.controller("userDefaultController",["$scope","userService","$uibModal","Notification","$q",function(e,t,r,a,i){e.updateCurrentUserProfile=function(e){return t.updateCurrentUserProfile(e).$promise},e.updateProfile=function(){e.userForm.$valid&&e.updateCurrentUserProfile(e.currentUser).then(function(e){a.success("Update successfully")})},e.changePassword=function(){var e=r.open({animation:!1,templateUrl:"app/main/user/dlg-change-password.html",controller:"dlgChangePasswordController",size:"md",windowClass:"passswordDialog"});e.result.then(function(e){},function(){})},e.validateBankNumber=function(r){var a=i.defer();return t.checkBankNumber({number:r,userName:e.currentUser.userName},function(e){e.result?a.resolve(e):a.reject(e)}),a.promise}}]),mainApp.controller("userRegisterController",["$scope","userService","Notification","$state","$q",function(e,t,r,a,i){e.init=function(){e.newUser={userInfo:{}},e.submitted=!1,e.step=1},e.init(),e.registerUser=function(){e.submitted=!0,e.regForm.$valid&&(e.newUser.userInfo.parentId=e.currentUser.userName,t.register(e.newUser,function(t){e.users=t,r.success("Register successfully"),e.step=2},function(e){r.error("Register failed")}))},e.endRegister=function(){a.reload()},e.interacted=function(t){return e.submitted||t.$dirty},e.validateBankNumber=function(e){var r=i.defer();return t.checkBankNumber({number:e,userName:"*"},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise},e.canIntroduce=function(){return null==e.currentUser||e.currentUser.userName.endsWith("A")||!e.currentUser.parentId}}]),mainApp.factory("userService",["$resource",function(e){return e(":path",{},{getCurrentUserContext:{method:"GET",params:{path:"api/user/getCurrentUserContext"}},getCurrentUserProfile:{method:"GET",params:{path:"api/user/getCurrent"}},getCurrentUserPinInfo:{method:"GET",params:{path:"api/user/getCurrentPin"}},getChildren:{method:"GET",params:{path:"api/user/getChildren",id:"id"},isArray:!0},register:{method:"POST",params:{path:"api/account/register",registerVM:"registerVM"},isArray:!0},updateCurrentUserProfile:{method:"POST",params:{path:"api/user/updateCurrent",userProfileVM:"userProfileVM"}},updateAvatar:{method:"POST",params:{path:"api/user/updateAvatar",avatar:"avatar"}},changePassword:{method:"POST",params:{path:"api/user/changePassword",model:"model"}},checkName:{method:"GET",params:{path:"api/user/checkName",name:"@name"}},checkBankNumber:{method:"GET",params:{path:"api/user/checkBankNumber",number:"@number",userName:"@userName"}},getCurrentAssociation:{method:"GET",params:{path:"api/user/getCurrentAssociation"},isArray:!0}})}]),mainApp.controller("userTreeController",["$scope","userService","_",function(e,t,r){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.getUserChildren=function(e){return t.getChildren({id:e}).$promise},e.init=function(){e.data=[],e.loadData()},e.loadData=function(){e.getCurrentUserProfile().then(function(t){e.data.push({id:t.id,title:t.displayName,name:t.userName})})},e.loadDataForSingleNode=function(t){t.isLoaded||(t.isLoaded=!0,e.getUserChildren(t.id).then(function(e){t.nodes=r.map(e,function(e){return{id:e.id,title:e.displayName,name:e.userName}})}))},e.init()}]);