function StringToXML(e){if(window.ActiveXObject){var t=new ActiveXObject("Microsoft.XMLDOM");return t.loadXML(e),t}return(new DOMParser).parseFromString(e,"text/xml")}var mainApp=angular.module("mainApp",["ui.router","ui.bootstrap","authApp","ngResource","ui.tree","smart-table","underscore","angularFileUpload","ngImgCrop","ngMessages","angular-loading-bar","ui.validate","reCAPTCHA","commonApp","ui-notification","timer"]);mainApp.config(["$stateProvider","$urlRouterProvider",function(e,t){t.otherwise("/login"),e.state("home",{url:"/",templateUrl:"app/main/home/home.html"}).state("login",{url:"/login",templateUrl:"app/authentication/login/login.html",controller:"loginController"}).state("resetPassword",{url:"/resetpassword",templateUrl:"app/authentication/password/resetPassword.html",controller:"resetPasswordController"}).state("refresh",{url:"/refresh",templateUrl:"app/authentication/refresh/refresh.html",controller:"refreshController"}).state("pin",{"abstract":!0,templateUrl:"app/common/templates/empty.html"}).state("pin.transfer",{url:"/transfer",templateUrl:"app/main/pin/pin.html",controller:"pinController"}).state("pin.rate",{url:"/rate",templateUrl:"app/main/pin/rate.html",controller:"rateController"}).state("user",{"abstract":!0,url:"/user",templateUrl:"app/main/user/user-info.html",controller:"userController"}).state("user.default",{url:"/",templateUrl:"app/main/user/user-info.default.html",controller:"userDefaultController"}).state("user.register",{url:"/register",templateUrl:"app/main/user/user-info.register.html",controller:"userRegisterController"}).state("user.tree",{url:"/tree",templateUrl:"app/main/user/user-info.tree.html",controller:"userTreeController"}).state("trade",{"abstract":!0,url:"/trade",templateUrl:"app/main/trade/trade-info.html",controller:"tradeInfoController"}).state("trade.statistic",{url:"/",templateUrl:"app/main/trade/trade-statistic.html",controller:"tradeStatisticController"}).state("trade.history",{url:"/history",templateUrl:"app/main/trade/trade-history.html",controller:"tradeHistoryController"}).state("waitinggiver",{url:"/waitinggiver",templateUrl:"app/main/account/waitingGiver.html",controller:"waitingGiverController"}).state("waitingreceiver",{url:"/waitingreceiver",templateUrl:"app/main/account/waitingReceiver.html",controller:"waitingReceiverController"}).state("account",{"abstract":!0,url:"/account",templateUrl:"app/main/account/account.html",controller:"accountController"}).state("account.default",{url:"/",templateUrl:"app/main/account/account-default.html",controller:function(){}}).state("account.details",{url:"/:key",templateUrl:"app/main/account/account-edit.html",controller:"accountInfoController"}).state("currentTransaction",{url:"/trade/current",templateUrl:"app/main/transaction/current.html",controller:"transactionCurrentController"}).state("reportedTransactions",{url:"/trade/reported-transactions",templateUrl:"app/main/transaction/transactionReported.html",controller:"transactionReportedController"}).state("association",{url:"/association",templateUrl:"app/main/association/association.html",controller:"associationController"})}]),mainApp.config(["$httpProvider",function(e){e.interceptors.push("authInterceptorService")}]),mainApp.run(["authService",function(e){e.fillAuthData()}]),mainApp.config(["treeConfig",function(e){e.defaultCollapsed=!0}]),mainApp.config(["cfpLoadingBarProvider",function(e){e.includeSpinner=!1}]),mainApp.config(["reCAPTCHAProvider","recaptchaSettings",function(e,t){e.setPublicKey(t.publicKey),e.setOptions({theme:"red"})}]),mainApp.config(["NotificationProvider",function(e){e.setOptions({delay:1500,startTop:20,startRight:10,verticalSpacing:20,horizontalSpacing:20,positionX:"right",positionY:"bottom",closeOnClick:!0,maxCount:3})}]),mainApp.factory("AccountState",function(){var e={Default:0,WaitGive:1,Gave:2,WaitReceive:3,InGiveTransaction:11,InReceiveTransaction:12,NotGive:21,NotConfirm:22,ReportedNotTransfer:23,AbadonedOne:31};return e.display=function(t){switch(t){case e.Default:return"Default";case e.WaitGive:return"Wait Give";case e.Gave:return"Gave";case e.WaitReceive:return"Wait Receive";case e.InGiveTransaction:return"In Give Transaction";case e.InReceiveTransaction:return"In Receive Transaction";case e.NotGive:return"Not Give";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";case e.AbadonedOne:return"Abadoned One";default:return""}},e}),mainApp.factory("PriorityLevel",function(){var e={Default:0,Priority:1,High:2,Highest:10};return e.display=function(t){switch(t){case e.Default:return"Default";case e.Priority:return"Priority";case e.High:return"High";case e.Highest:return"Highest";default:return""}},e}),mainApp.factory("TransactionState",function(){var e={Begin:0,Transfered:1,Confirmed:2,NotTransfer:21,NotConfirm:22,ReportedNotTransfer:23,Abadoned:31,Failed:51};return e.display=function(t){switch(t){case e.Begin:return"Begin";case e.Transfered:return"Transfered";case e.Confirmed:return"Confirmed";case e.NotTransfer:return"Not Transfer";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";case e.Abadoned:return"Abadoned";case e.Failed:return"Failed";default:return""}},e}),mainApp.factory("TransactionType",function(){var e={Begin:0,Abadoned:31,Replacement:41};return e.display=function(t){switch(t){case e.Begin:return"Begin";case e.Abadoned:return"Abadoned";case e.Replacement:return"Replacement";default:return""}},e}),mainApp.factory("ReportResult",function(){var e={Default:0,GiverTrue:1,ReceiverTrue:2,BothTrue:11,BothFalse:12};return e.display=function(t){switch(t){case e.Default:return"Default";case e.GiverTrue:return"Giver True";case e.ReceiverTrue:return"Receiver True";case e.BothTrue:return"Both True";case e.BothFalse:return"Both False";default:return""}},e}),mainApp.factory("UserState",function(){var e={Default:0,NotGive:21,NotConfirm:22};return e.display=function(t){switch(t){case e.Default:return"Default";case e.NotGive:return"Not Give";case e.NotConfirm:return"Not Confirm";default:return""}},e}),mainApp.factory("ConfigData",function(){var e={dateFormat:"yyyy/MM/dd",dateTimeFormat:"yyyy/MM/dd - hh:mm:ss"};return e}),mainApp.filter("yesno",[function(){return function(e){return e?"Yes":"No"}}]),mainApp.config(["stConfig","filterSetting",function(e,t){e.pagination.itemsByPage=t.pagination.itemsByPage,e.pagination.displayedPages=t.pagination.displayedPages}]),mainApp.controller("accountController",["$scope","$state","accountService","Notification","AccountState","PriorityLevel","UserState","ConfigData",function(e,t,r,a,n,i,o,s){e.loadData=function(t){console.log(t),e.data=[],r.queryAccount(JSON.stringify(t),function(r){e.data=r.data,t.pagination.numberOfPages=Math.ceil(r.totalItems/t.pagination.number)})},e.reload=function(){e.loadData()},e.viewDefails=function(r){e.current&&(e.current.selected=!1),r.selected=!0,e.current=r,t.go("^.details",{key:r.userName})},e.getAccountState=function(e){var t=e.UserState==o.NotGive||e.UserState==o.NotConfirm?o.display(e.userState)+" ("+e.relatedAccount+")":n.display(e.state);return t},e.init=function(){e.AccountState=n,e.PriorityLevel=i,e.UserState=o,e.ConfigData=s},e.init()}]),mainApp.controller("accountInfoController",["$scope","$stateParams","accountService","Notification","AccountState","PriorityLevel","UserState",function(e,t,r,a,n,i,o){e.getData=function(){r.status({key:t.key},{key:t.key},function(t){e.info=t,e.info.newPriority=e.info.priority,e.info.newState=e.info.state})},e.queueGive=function(){e.info.isAllowGive=!1,r.queueGive({key:e.info.userName},{key:e.info.userName},function(t){a.success("Queue give successful!"),e.getData()},function(t){e.info.isAllowGive=!0,a.success(t)})},e.queueReceive=function(){e.info.isAllowReceive=!1,r.queueReceive({key:e.info.userName},{key:e.info.userName},function(t){a.success("Queue receive successful!"),e.getData()},function(t){e.info.isAllowReceive=!0,a.success(t)})},e.setPriority=function(){r.setAccountPriority({userName:e.info.userName,priority:e.info.newPriority},function(e){a.success("Priority Changed!")})},e.setAccountState=function(){r.setAccountState({userName:e.info.userName,state:e.info.newState},function(t){a.success("State Changed!"),e.getData()})},e.init=function(){e.UserState=o,e.AccountState=n,e.accountStateToSelect=[{value:n.Default,text:"Giveable"},{value:n.Gave,text:"Receivable"}],e.PriorityLevel=i,e.priorityToSelect=[{value:i.Default,text:i.display(i.Default)},{value:i.Priority,text:i.display(i.Priority)},{value:i.High,text:i.display(i.High)},{value:i.Highest,text:i.display(i.Highest)}],e.getData()},e.init()}]),mainApp.factory("accountService",["$resource",function(e){return e(":path",{},{status:{method:"POST",params:{path:"api/trade/accountStatus",key:"key"}},setAccountPriority:{method:"POST",params:{path:"api/trade/setAccountPriority"}},setAccountState:{method:"POST",params:{path:"api/trade/setAccountState"}},queueGive:{method:"POST",params:{path:"api/trade/queueGive",key:"key"}},queueReceive:{method:"POST",params:{path:"api/trade/queueReceive",key:"key"}},queryAccount:{method:"POST",params:{path:"api/trade/queryAccount"}},queryHistory:{method:"POST",params:{path:"api/trade/queryHistory",key:"key"},isArray:!0},queryWaitingGivers:{method:"POST",params:{path:"api/trade/queryWaitingGivers"},isArray:!0},queryWaitingReceivers:{method:"POST",params:{path:"api/trade/queryWaitingReceivers"},isArray:!0}})}]),mainApp.controller("waitingGiverController",["$scope","$state","accountService","Notification","PriorityLevel","ConfigData",function(e,t,r,a,n,i){e.loadData=function(){e.data=[],r.queryWaitingGivers({},function(t){e.data=t})},e.reload=function(){e.loadData()},e.viewDefails=function(t){e.target&&(e.target.selected=!1),t.selected=!0,e.target=t,e.selected=!0},e.init=function(){e.PriorityLevel=n,e.ConfigData=i,e.data=[],e.target={},e.selected=!1,e.loadData()},e.init()}]),mainApp.controller("waitingReceiverController",["$scope","$state","accountService","Notification","PriorityLevel","ConfigData",function(e,t,r,a,n,i){e.loadData=function(){e.data=[],r.queryWaitingReceivers({},function(t){e.data=t})},e.reload=function(){e.loadData()},e.viewDefails=function(t){e.target&&(e.target.selected=!1),t.selected=!0,e.target=t,e.selected=!0},e.init=function(){e.PriorityLevel=n,e.ConfigData=i,e.data=[],e.target={},e.selected=!1,e.loadData()},e.init()}]),mainApp.controller("associationController",["$scope","userService","authService","$state",function(e,t,r,a){e.init=function(){e.associatedUsrs=[],e.selectedUsr={}},e.init(),e.getAssociation=function(){t.getCurrentAssociation(function(t){e.associatedUsrs=t})},e.getAssociation(),e["switch"]=function(t){e.selectedUsr.id!=t.id&&(e.password="",e.selectedUsr=t)},e.login=function(){var t={userName:e.selectedUsr.userName,password:e.selectedUsr.password,useRefreshTokens:!1};r.login(t).then(function(e){a.go("home")},function(t){e.message="Wrong password!"})},e.isMain=function(t){return null==t||t.userName.endsWith("A")||1==e.associatedUsrs.length}}]),mainApp.controller("indexController",["$scope","$state","authService","userService","$location","_",function(e,t,r,a,n,i){e.logOut=function(){r.logOut(),t.go("login")},e.authentication=r.authentication,e.$on("user:authenticated",function(t,r){e.getUserContext()}),e.$on("user:updateAvatar",function(t,r){e.userContext.avatar.url=r}),e.getUserContext=function(){e.userContext={},a.getCurrentUserContext(function(t){e.userContext=t})};var o=n.search();e.authentication.isAuth&&"true"!=o.anonymous&&e.getUserContext(),e.forAdmin=function(){return e.userContext&&i.contains(e.userContext.roles,"superadmin")}}]),mainApp.controller("pinController",["$scope","userService","pinService","$q","Notification","$state","$window",function(e,t,r,a,n,i,o){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={step:1}},e.init(),e.processToConfirm=function(){e.submitted=!0,e.frmTransfer.$valid&&(e.transaction.step=2)},e.goBack=function(){e.transaction.step=1},e.transferPIN=function(){r.transfer(e.transaction,function(e){n.success("Transaction has been completed successfully"),i.reload()},function(t){n.error("Some errors happenned"),e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2,o.Recaptcha.reload())})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(r){var n=a.defer();return t.checkName({name:r},function(t){t.isSuccessful?(e.toUser=t.result,n.resolve(t)):(e.toUser={},n.reject(t))}),n.promise}}]),mainApp.factory("pinService",["$resource",function(e){return e(":path",{},{transfer:{method:"POST",params:{path:"api/pin/transfer",transactionVM:"transactionVM"}},getCurrentUserHistory:{method:"GET",params:{path:"api/pin/getAll"},isArray:!0}})}]),mainApp.controller("rateController",["$scope","$q","_","$http",function(e,t,r,a){e.rate={},e.bitPrices=[],e.init=function(){e.bitPrices[0]={},e.bitPrices[1]={},e.bitPrices[2]={},e.bitPrices[3]={},e.bitPrices[4]={},e.bitPrices[5]={},e.bitPrices[6]={},e.bitPrices[0].quantity1=1,e.bitPrices[0].quantity2=50,e.bitPrices[1].quantity1=51,e.bitPrices[1].quantity2=100,e.bitPrices[2].quantity1=101,e.bitPrices[2].quantity2=200,e.bitPrices[3].quantity1=201,e.bitPrices[3].quantity2=300,e.bitPrices[4].quantity1=301,e.bitPrices[4].quantity2=400,e.bitPrices[5].quantity1=401,e.bitPrices[5].quantity2=1e3,e.bitPrices[6].quantity1=1001,e.bitPrices[6].quantity2="~",e.bitPrices[0].usd=9.09,e.bitPrices[1].usd=8.18,e.bitPrices[2].usd=7.27,e.bitPrices[3].usd=6.36,e.bitPrices[4].usd=5.45,e.bitPrices[5].usd=4.55,e.bitPrices[6].usd=3.64,a({method:"GET",url:"http://api.coindesk.com/v1/bpi/currentprice.json",skipInterceptor:!0}).then(function(t){e.rate.usd=t.data.bpi.USD.rate_float;for(var r=0;r<e.bitPrices.length;r++)e.bitPrices[r].bitCoin=e.bitPrices[r].usd/e.rate.usd})},e.init()}]),mainApp.controller("tradeHistoryController",["$scope","$uibModal","tradeService","Notification","AccountState","ConfigData",function(e,t,r,a,n,i){e.loadData=function(){e.data=[],r.queryHistory({},function(t){e.data=t})},e.viewDefails=function(e){var r=t.open({templateUrl:"app/main/transaction/transactionHistory.html",size:"lg",controller:"transactionHistoryController",resolve:{targetData:function(){return e}}});r.result.then(function(e){},function(){})},e.init=function(){e.AccountState=n,e.ConfigData=i,e.data=[],e.loadData()},e.init()}]),mainApp.controller("tradeInfoController",["$scope","tradeService","Notification","AccountState","PriorityLevel","UserState",function(e,t,r,a,n,i){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){e.info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(t){e.info.isAllowGive=!0,r.success(t)})},e.queueReceive=function(){e.info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(t){e.info.isAllowReceive=!0,r.success(t)})},e.init=function(){e.AccountState=a,e.UserState=i,e.PriorityLevel=n,e.updateStatus()},e.init()}]),mainApp.factory("tradeService",["$resource",function(e){return e(":path",{},{status:{method:"POST",params:{path:"api/trade/accountStatus"}},queueGive:{method:"POST",params:{path:"api/trade/queueGive"}},queueReceive:{method:"POST",params:{path:"api/trade/queueReceive"}},queryHistory:{method:"POST",params:{path:"api/trade/queryHistory"},isArray:!0}})}]),mainApp.controller("tradeStatisticController",["$scope","tradeService","Notification","AccountState","PriorityLevel",function(e,t,r,a,n){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(e){info.isAllowGive=!0,r.success(e)})},e.queueReceive=function(){info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(e){info.isAllowReceive=!0,r.success(e)})},e.init=function(){e.AccountState=a,e.PriorityLevel=n,e.updateStatus()},e.init()}]),mainApp.controller("errorDlgController",["$scope","$uibModalInstance","rejection",function(e,t,r){function a(){r&&r.data&&(e.isJson=angular.isObject(r.data))}e.cancel=function(){t.close()},e.rejection=r,e.isCollapsed=!1,e.isHtml=!1,e.isJson=!1,a()}]),mainApp.controller("importPopupController",["$scope","$window","$uibModal","$log","$uibModalInstance","_","FileUploader","cfpLoadingBar","localStorageService","targetData",function(e,t,r,a,n,i,o,s,c,u){e.target={},e.data=[],e.uploader=new o({url:u.uploadLink}),e.upload=function(){e.isError=!1,e.uploaded=!1,e.importing=!0,s.start();var t=i.last(e.uploader.queue);t.upload()},e.ok=function(e){console.log(e);var t=e.url;t=t.replace("~/",""),n.close(t)},e.cancel=function(){n.dismiss("cancel")},e.uploaded=!1,e.isError=!1,e.importing=!1,e.uploader.queueLimit=2,e.uploader.filters.push({name:"size",fn:function(e){return e.size<=5242880}}),e.uploader.filters.push({name:"image",fn:function(t){return e.uploader.hasHTML5?/\/(png|jpeg|jpg)$/.test(t.file.type):!0}});var l=c.get("authorizationData");l&&(e.uploader.headers.Authorization="Bearer "+l.token),e.uploader.onAfterAddingFile=function(t){e.uploader.getIndexOfItem(t)&&e.uploader.removeFromQueue(0)},e.uploader.onErrorItem=function(t,a,n,i){e.isError=!0,e.importing=!1,s.complete();r.open({templateUrl:"app/main/transaction/errorDlgHandled.html",size:"lg",controller:"errorDlgController",resolve:{rejection:function(){return{data:a,statusText:"File is invalid. See techinical details for more information"}}}})},e.uploader.onSuccessItem=function(t,r,a,n){e.uploaded=!0,e.importing=!1,s.complete(),e.ok(r)}}]),mainApp.controller("transactionCurrentController",["$scope","_","$timeout","$uibModal","$window","transactionService","tradeService","Notification","AccountState","TransactionState","ConfigData",function(e,t,r,a,n,i,o,s,c,u,l){function p(){switch(e.info.state){case c.Default:break;case c.AbadonOne:break;case c.WaitGive:e.overviewState.queued=1,e.overviewState.give=0;break;case c.InGiveTransaction:e.overviewState.queued=1,e.overviewState.give=0;break;case c.Gave:e.overviewState.queued=1,e.overviewState.give=1,e.overviewState.waitCofirm=1,e.overviewState.receive=-1;break;case c.WaitReceive:e.overviewState.queued=1,e.overviewState.give=1,e.overviewState.waitCofirm=1,e.overviewState.receive=0;break;case c.InReceiveTransaction:e.overviewState.queued=1,e.overviewState.give=1,e.overviewState.waitCofirm=1,e.overviewState.receive=0;break;case c.NotGive:e.overviewState.queued=-1;break;case c.NotConfirm:e.overviewState.queued=-1;break;case c.ReportedNotTransfer:e.overviewState.queued=-1}console.log(e.overviewState)}function d(){e.info.state==c.InGiveTransaction?(e.accountDisplayTemplate=e.receiverInfoTemplateUrl,e.grState="giving",i.giveRequested({},function(e){f(e)})):e.info.state==c.InReceiveTransaction&&(e.accountDisplayTemplate=e.giverInfoTemplateUrl,e.grState="receiving",i.receiveRequested({},function(e){f(e)}))}function f(r){t.each(r,function(t){t.isAllowConfirmGave=t.state==u.Begin,t.isAllowConfirmReceived=t.state==u.Transfered,t.isAllowAbadonTransaction=e.info.isAllowAbadonTransaction&&t.state==u.Begin,t.isAllowAttachment=t.state!=u.Abadoned,t.isAllowUploadAttachment=t.state==u.Begin||t.state==u.Transfered,t.isAbadoned=t.state==u.Abadoned,m(t)}),e.transactions=r}function m(r){e.histories.push({userName:"System",rating:5,time:r.created,action:"Created"});var a="";"giving"==e.grState?(a=r.receiverId,r.receivedDate&&e.histories.push({userName:a,rating:r.rating,time:r.receivedDate,action:"Received"})):(a=r.giverId,r.transferedDate&&e.histories.push({userName:a,rating:r.rating,time:r.transferedDate,action:"Transfered"})),e.histories=t.sortBy(e.histories,function(e){return e.time})}e.updateStatus=function(){return o.status({},function(t){e.info=t,p()}).$promise},e.queueGive=function(){e.info.isAllowGive=!1,o.queueGive({},function(t){s.success("Queue give successful!"),e.updateStatus()},function(t){e.info.isAllowGive=!0,s.success(t)})},e.queueReceive=function(){e.info.isAllowReceive=!1,o.queueReceive({},function(t){s.success("Queue receive successful!"),e.updateStatus()},function(t){e.info.isAllowReceive=!0,s.success(t)})},e.moneyTransfered=function(t){i.moneyTransfered(t,function(r){s.success("Money Transfered");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),d()})},e.moneyReceived=function(t){i.moneyReceived(t,function(r){s.success("Money Received");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),d()})},e.reportNotTransfer=function(t){i.reportNotTransfer(t,function(r){s.success("Transaction Reported");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),d()})},e.updateImg=function(e){i.updateImg(e,function(e){s.success("Upload saved.")})},e.upload=function(t){var r=a.open({templateUrl:"app/main/transaction/importPopup.html",size:"lg",controller:"importPopupController",resolve:{targetData:function(){return{uploadLink:"api/transaction/upload"}}}});r.result.then(function(r){s.success("Upload successful.");n.StringToXML(r);t.attachmentUrl=r,t.attachmentUrl=t.attachmentUrl.replace("~/",""),e.updateImg(t)},function(){s.success("Upload error.")})},e.abadon=function(t){i.abadonTransaction(t,function(r){s.success("Transaction Abadoned");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),d()})},e.init=function(){e.grState="",e.giverInfoTemplateUrl="app/main/transaction/info-giver.html",e.receiverInfoTemplateUrl="app/main/transaction/info-receiver.html",e.TransactionState=u,e.AccountState=c,e.ConfigData=l,e.overviewState={queued:0,give:-1,waitCofirm:-1,receive:-1,ended:-1},e.histories=[],e.updateStatus().then(function(e){d()})},e.init()}]),mainApp.controller("transactionHistoryController",["$scope","_","$timeout","$uibModal","$uibModalInstance","transactionService","tradeService","Notification","ConfigData","AccountState","TransactionState","targetData",function(e,t,r,a,n,i,o,s,c,u,l,p){function d(r){t.each(r,function(e){e.isAllowConfirmGave=e.state==l.Begin,e.isAllowConfirmReceived=e.state==l.Transfered,e.isAllowAbadonTransaction=!1,e.isAllowAttachment=e.state!=l.Abadoned,e.isAllowUploadAttachment=e.state==l.Begin||e.state==l.Transfered,e.isAbadoned=e.state==l.Abadoned,f(e)}),e.transactions=r,m()}function f(r){e.histories.push({userName:"System",rating:5,time:r.created,action:"Created"});var a="";"giving"==e.grState?(a=r.receiverId,r.receivedDate&&e.histories.push({userName:a,rating:r.rating,time:r.receivedDate,action:"Received"})):(a=r.giverId,r.transferedDate&&e.histories.push({userName:a,rating:r.rating,time:r.transferedDate,action:"Transfered"})),e.histories=t.sortBy(e.histories,function(e){return e.time})}function m(){var r=t.every(e.transactions,function(e){return e.state==l.Confirmed});"giving"==e.grState?(e.overviewState.queued=1,r?(e.overviewState.give=1,e.overviewState.waitCofirm=1):e.overviewState.give=0):"receiving"==e.grState&&(e.overviewState.queued=1,e.overviewState.give=1,e.overviewState.waitCofirm=1,r?(e.overviewState.receive=1,e.overviewState.ended=1):e.overviewState.receive=0),console.log(e.overviewState)}e.getCurrentTransactions=function(){e.targetData.type==u.InGiveTransaction?(e.accountDisplayTemplate=e.receiverInfoTemplateUrl,e.grState="giving"):e.targetData.type==u.InReceiveTransaction&&(e.accountDisplayTemplate=e.giverInfoTemplateUrl,e.grState="receiving"),i.history(p,function(e){d(e)})},e.moneyTransfered=function(t){i.moneyTransfered(t,function(r){s.success("Money Transfered");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),getCurrentTransactions()})},e.moneyReceived=function(t){i.moneyReceived(t,function(r){s.success("Money Received");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),getCurrentTransactions()})},e.reportNotTransfer=function(t){i.reportNotTransfer(t,function(r){s.success("Transaction Reported");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),getCurrentTransactions()})},e.updateImg=function(e){i.updateImg(e,function(e){s.success("Upload saved.")})},e.upload=function(t){var r=a.open({templateUrl:"app/main/transaction/importPopup.html",size:"lg",controller:"importPopupController",resolve:{targetData:function(){return{uploadLink:"api/transaction/upload"}}}});r.result.then(function(r){s.success("Upload successful."),t.attachmentUrl=r,e.updateImg(t)},function(){s.success("Upload error.")})},e.abadon=function(t){i.abadonTransaction(t,function(r){s.success("Transaction Abadoned");var a=e.transactions.indexOf(t);-1!==a&&(e.transactions[a]=r),e.updateStatus(),getCurrentTransactions()})},e.init=function(){e.targetData=p,e.grState="",e.giverInfoTemplateUrl="app/main/transaction/info-giver.html",e.receiverInfoTemplateUrl="app/main/transaction/info-receiver.html",e.TransactionState=l,e.AccountState=u,e.ConfigData=c,e.overviewState={queued:0,give:-1,waitCofirm:-1,receive:-1,ended:-1},e.histories=[],e.getCurrentTransactions()},e.init()}]),mainApp.controller("transactionReportedController",["$scope","$state","transactionService","Notification","TransactionType","ReportResult","ConfigData",function(e,t,r,a,n,i,o){function s(t){e.target.result=t,r.applyReport(e.target,function(t){a.success("Transaction Applied."),e.reload()})}e.loadData=function(){e.data=[],r.reportedTransactions({},function(t){e.data=t})},e.reload=function(){e.target={},e.loadData()},e.viewDefails=function(t){e.target&&(e.target.selected=!1),t.selected=!0,e.target=t,e.selected=!0},e.giverTrue=function(){s(i.GiverTrue)},e.receiverTrue=function(){s(i.ReceiverTrue)},e.bothTrue=function(){s(i.BothTrue)},e.bothFalse=function(){s(i.BothFalse)},e.bothTrue=function(){},e.init=function(){e.ConfigData=o,e.TransactionType=n,e.ReportResult=i,e.data=[],e.target={},e.selected=!1,e.loadData()},e.init()}]),mainApp.factory("transactionService",["$resource",function(e){return e(":path",{},{history:{method:"POST",params:{path:"api/transaction/history",key:"key"},isArray:!0},giveRequested:{method:"POST",params:{path:"api/transaction/giveRequested"},isArray:!0},receiveRequested:{method:"POST",params:{path:"api/transaction/receiveRequested"},isArray:!0},moneyTransfered:{method:"POST",params:{path:"api/transaction/moneyTransfered"}},moneyReceived:{method:"POST",params:{path:"api/transaction/moneyReceived"}},reportNotTransfer:{method:"POST",params:{path:"api/transaction/reportNotTransfer"}},updateImg:{method:"POST",params:{path:"api/transaction/updateImg"}},abadonTransaction:{method:"POST",params:{path:"api/transaction/abadonTransaction"}},reportedTransactions:{method:"POST",params:{path:"api/transaction/reportedTransactions"},isArray:!0},applyReport:{method:"POST",params:{path:"api/transaction/applyReport"}}})}]),mainApp.controller("dlgChangeAvatarController",["$scope","userService","FileUploader","_","localStorageService","$uibModalInstance","Notification",function(e,t,r,a,n,i,o){e.uploadImage=function(){var e=a.last(s.queue);e.upload()};var s=e.uploader=new r({autoUpload:!1,url:"api/user/updateAvatar"});s.filters.push({name:"size",fn:function(e){return e.size<=5242880}}),s.filters.push({name:"image",fn:function(e){return s.hasHTML5?/\/(png|jpeg|jpg)$/.test(e.file.type):!0}});var c=n.get("authorizationData");c&&(s.headers.Authorization="Bearer "+c.token),s.onAfterAddingFile=function(t){e.cropped={image:""};var r=new FileReader;r.onload=function(t){e.$apply(function(){e.image=t.target.result})},r.readAsDataURL(t._file)},s.onBeforeUploadItem=function(t){var r=u(e.cropped.image);t._file=r};var u=function(e){var t;t=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):decodeURI(e.split(",")[1]);for(var r=e.split(",")[0].split(":")[1].split(";")[0],a=[],n=0;n<t.length;n++)a.push(t.charCodeAt(n));return new Blob([new Uint8Array(a)],{type:r})};s.onSuccessItem=function(t,r,a,n){console.log(r),e.ok(r)},e.ok=function(e){o.success("Avatar has been updated"),i.close(e)},e.cancel=function(){i.dismiss()}}]),mainApp.controller("dlgChangePasswordController",["$scope","userService","$uibModalInstance","Notification",function(e,t,r,a){e.interacted=function(t){return e.submitted||t.$dirty},e.init=function(){e.failed=!1,e.submitted=!1,e.model={}},e.init(),e.changePassword=function(e){return t.changePassword(e).$promise},e.submit=function(){e.submitted=!0,e.frmPassword.$valid&&e.changePassword(e.model).then(function(t){t.result?e.ok():(e.failed=!0,a.error("Failed to update your password"))})},e.ok=function(e){a.success("Password has been changed successfully"),r.close(e)},e.cancel=function(){r.dismiss()}}]),mainApp.controller("userController",["$scope","userService","imageService","$uibModal",function(e,t,r,a){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.init=function(){e.currentUser=e.currentUser||null,null==e.currentUser&&e.getCurrentUserProfile().then(function(t){e.currentUser=t})},e.init(),e.updateAvatar=function(){var t=a.open({animation:!1,templateUrl:"app/main/user/dlg-change-avatar.html",controller:"dlgChangeAvatarController",size:"lg",windowClass:"portraitDialog"});t.result.then(function(t){e.currentUser=t,e.currentUser.avatar.url=e.currentUser.avatar.url+"?"+(new Date).getTime(),e.$emit("user:updateAvatar",e.currentUser.avatar.url)},function(){})}}]),mainApp.controller("userDefaultController",["$scope","userService","$uibModal","Notification","$q",function(e,t,r,a,n){e.updateCurrentUserProfile=function(e){return t.updateCurrentUserProfile(e).$promise},e.updateProfile=function(){e.userForm.$valid&&e.updateCurrentUserProfile(e.currentUser).then(function(e){a.success("Update successfully")})},e.changePassword=function(){var e=r.open({animation:!1,templateUrl:"app/main/user/dlg-change-password.html",controller:"dlgChangePasswordController",size:"md",windowClass:"passswordDialog"});e.result.then(function(e){},function(){})},e.validateBankNumber=function(r){var a=n.defer();return t.checkBankNumber({number:r,userName:e.currentUser.userName},function(e){e.result?a.resolve(e):a.reject(e)}),a.promise}}]),mainApp.controller("userRegisterController",["$scope","userService","Notification","$state","$q",function(e,t,r,a,n){e.init=function(){e.newUser={userInfo:{}},e.submitted=!1,e.step=1},e.init(),e.registerUser=function(){e.submitted=!0,e.regForm.$valid&&(e.newUser.userInfo.parentId=e.currentUser.userName,t.register(e.newUser,function(t){e.users=t,r.success("Register successfully"),e.step=2},function(e){r.error("Register failed")}))},e.endRegister=function(){a.reload()},e.interacted=function(t){return e.submitted||t.$dirty},e.validateBankNumber=function(e){var r=n.defer();return t.checkBankNumber({number:e,userName:"*"},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise},e.canIntroduce=function(){return null==e.currentUser||e.currentUser.userName.endsWith("A")||!e.currentUser.parentId}}]),mainApp.factory("userService",["$resource",function(e){return e(":path",{},{getCurrentUserContext:{method:"GET",params:{path:"api/user/getCurrentUserContext"}},getCurrentUserProfile:{method:"GET",params:{path:"api/user/getCurrent"}},getCurrentUserPinInfo:{method:"GET",params:{path:"api/user/getCurrentPin"}},getChildren:{method:"GET",params:{path:"api/user/getChildren",id:"id"},isArray:!0},register:{method:"POST",params:{path:"api/account/register",registerVM:"registerVM"},isArray:!0},updateCurrentUserProfile:{method:"POST",params:{path:"api/user/updateCurrent",userProfileVM:"userProfileVM"}},updateAvatar:{method:"POST",params:{path:"api/user/updateAvatar",avatar:"avatar"}},changePassword:{method:"POST",params:{path:"api/user/changePassword",model:"model"}},checkName:{method:"GET",
params:{path:"api/user/checkName",name:"@name"}},checkBankNumber:{method:"GET",params:{path:"api/user/checkBankNumber",number:"@number",userName:"@userName"}},getCurrentAssociation:{method:"GET",params:{path:"api/user/getCurrentAssociation"},isArray:!0}})}]),mainApp.controller("userTreeController",["$scope","userService","_",function(e,t,r){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.getUserChildren=function(e){return t.getChildren({id:e}).$promise},e.init=function(){e.data=[],e.loadData()},e.loadData=function(){e.getCurrentUserProfile().then(function(t){e.data.push({id:t.id,title:t.displayName,name:t.userName})})},e.loadDataForSingleNode=function(t){t.isLoaded||(t.isLoaded=!0,e.getUserChildren(t.id).then(function(e){t.nodes=r.map(e,function(e){return{id:e.id,title:e.displayName,name:e.userName}})}))},e.init()}]);