<<<<<<< HEAD
var mainApp=angular.module("mainApp",["ui.router","ui.bootstrap","authApp","ngResource","ui.tree","smart-table","underscore","angularFileUpload","ngImgCrop","ngMessages","angular-loading-bar","ui.validate","reCAPTCHA","commonApp","ui-notification"]);mainApp.config(["$stateProvider","$urlRouterProvider",function(e,r){r.otherwise("/login"),e.state("home",{url:"/",templateUrl:"app/main/home/home.html"}).state("login",{url:"/login",templateUrl:"app/authentication/login/login.html",controller:"loginController"}).state("refresh",{url:"/refresh",templateUrl:"app/authentication/refresh/refresh.html",controller:"refreshController"}).state("pin",{url:"/pin",templateUrl:"app/main/pin/pin.html",controller:"pinController"}).state("user",{"abstract":!0,url:"/user",templateUrl:"app/main/user/user-info.html",controller:"userController"}).state("user.default",{url:"/",templateUrl:"app/main/user/user-info.default.html",controller:"userDefaultController"}).state("user.register",{url:"/register",templateUrl:"app/main/user/user-info.register.html",controller:"userRegisterController"}).state("user.tree",{url:"/tree",templateUrl:"app/main/user/user-info.tree.html",controller:"userTreeController"}).state("association",{url:"/association",templateUrl:"app/main/association/association.html",controller:"associationController"})}]),mainApp.config(["$httpProvider",function(e){e.interceptors.push("authInterceptorService")}]),mainApp.run(["authService",function(e){e.fillAuthData()}]),mainApp.config(["treeConfig",function(e){e.defaultCollapsed=!0}]),mainApp.config(["cfpLoadingBarProvider",function(e){e.includeSpinner=!1}]),mainApp.config(["reCAPTCHAProvider","recaptchaSettings",function(e,r){e.setPublicKey(r.publicKey),e.setOptions({theme:"red"})}]),mainApp.config(["NotificationProvider",function(e){e.setOptions({delay:1500,startTop:20,startRight:10,verticalSpacing:20,horizontalSpacing:20,positionX:"right",positionY:"bottom",closeOnClick:!0,maxCount:3})}]),mainApp.controller("associationController",["$scope","userService","authService","$state",function(e,r,t,n){e.init=function(){e.associatedUsrs=[],e.selectedUsr={}},e.init(),e.getAssociation=function(){r.getCurrentAssociation(function(r){e.associatedUsrs=r})},e.getAssociation(),e["switch"]=function(r){e.selectedUsr.id!=r.id&&(e.password="",e.selectedUsr=r)},e.login=function(){var r={userName:e.selectedUsr.userName,password:e.selectedUsr.password,useRefreshTokens:!1};t.login(r).then(function(e){n.go("home")},function(r){e.message="Wrong password!"})},e.isMain=function(r){return null==r||r.userName.endsWith("A")||1==e.associatedUsrs.length}}]),mainApp.controller("indexController",["$scope","$state","authService","userService",function(e,r,t,n){e.logOut=function(){t.logOut(),r.go("login")},e.authentication=t.authentication,e.$on("user:authenticated",function(r,t){e.getUserContext()}),e.$on("user:updateAvatar",function(r,t){e.userContext.avatar.url=t}),e.getUserContext=function(){n.getCurrentUserContext(function(r){e.userContext=r})},e.getUserContext()}]),mainApp.controller("pinController",["$scope","userService","pinService","$q","Notification","$state",function(e,r,t,n,a,i){e.getCurrentUserPinInfo=function(){return r.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return t.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(r){e.currentPinBalance=r}),e.getCurrentUserPinTransactionHistory().then(function(r){e.transactionHistories=r}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&t.transfer(e.transaction,function(e){a.success("Transaction has been completed successfully"),i.reload()},function(r){e.failed=1,"invalid_captcha"==r.data.message&&(e.failed=2)})},e.interacted=function(r){return e.submitted||r.$dirty},e.validateUser=function(e){var t=n.defer();return r.checkName({name:e},function(e){e.result?t.resolve(e):t.reject(e)}),t.promise}}]),mainApp.factory("pinService",["$resource",function(e){return e(":path",{},{transfer:{method:"POST",params:{path:"api/pin/transfer",transactionVM:"transactionVM"}},getCurrentUserHistory:{method:"GET",params:{path:"api/pin/getAll"},isArray:!0}})}]),mainApp.controller("dlgChangeAvatarController",["$scope","userService","FileUploader","_","localStorageService","$uibModalInstance","Notification",function(e,r,t,n,a,i,o){e.uploadImage=function(){var e=n.last(s.queue);e.upload()};var s=e.uploader=new t({autoUpload:!1,url:"api/user/updateAvatar"});s.filters.push({name:"size",fn:function(e){return e.size<=5242880}}),s.filters.push({name:"image",fn:function(e){return s.hasHTML5?/\/(png|jpeg|jpg)$/.test(e.file.type):!0}});var u=a.get("authorizationData");u&&(s.headers.Authorization="Bearer "+u.token),s.onAfterAddingFile=function(r){e.cropped={image:""};var t=new FileReader;t.onload=function(r){e.$apply(function(){e.image=r.target.result})},t.readAsDataURL(r._file)},s.onBeforeUploadItem=function(r){var t=l(e.cropped.image);r._file=t};var l=function(e){var r;r=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):decodeURI(e.split(",")[1]);for(var t=e.split(",")[0].split(":")[1].split(";")[0],n=[],a=0;a<r.length;a++)n.push(r.charCodeAt(a));return new Blob([new Uint8Array(n)],{type:t})};s.onSuccessItem=function(r,t,n,a){console.log(t),e.ok(t)},e.ok=function(e){o.success("Avatar has been updated"),i.close(e)},e.cancel=function(){i.dismiss()}}]),mainApp.controller("dlgChangePasswordController",["$scope","userService","$uibModalInstance","Notification",function(e,r,t,n){e.interacted=function(r){return e.submitted||r.$dirty},e.init=function(){e.failed=!1,e.submitted=!1,e.model={}},e.init(),e.changePassword=function(e){return r.changePassword(e).$promise},e.submit=function(){e.submitted=!0,e.frmPassword.$valid&&e.changePassword(e.model).then(function(r){r.result?e.ok():(e.failed=!0,n.error("Failed to update your password"))})},e.ok=function(e){n.success("Password has been changed successfully"),t.close(e)},e.cancel=function(){t.dismiss()}}]),mainApp.controller("userController",["$scope","userService","imageService","$uibModal",function(e,r,t,n){e.getCurrentUserProfile=function(){return r.getCurrentUserProfile().$promise},e.init=function(){e.currentUser=e.currentUser||null,null==e.currentUser&&e.getCurrentUserProfile().then(function(r){e.currentUser=r})},e.init(),e.updateAvatar=function(){var r=n.open({animation:!1,templateUrl:"app/main/user/dlg-change-avatar.html",controller:"dlgChangeAvatarController",size:"lg",windowClass:"portraitDialog"});r.result.then(function(r){e.currentUser=r,e.currentUser.avatar.url=e.currentUser.avatar.url+"?"+(new Date).getTime(),e.$emit("user:updateAvatar",e.currentUser.avatar.url)},function(){})}}]),mainApp.controller("userDefaultController",["$scope","userService","$uibModal","Notification","$q",function(e,r,t,n,a){e.updateCurrentUserProfile=function(e){return r.updateCurrentUserProfile(e).$promise},e.updateProfile=function(){e.userForm.$valid&&e.updateCurrentUserProfile(e.currentUser).then(function(e){n.success("Update successfully")})},e.changePassword=function(){var e=t.open({animation:!1,templateUrl:"app/main/user/dlg-change-password.html",controller:"dlgChangePasswordController",size:"md",windowClass:"passswordDialog"});e.result.then(function(e){},function(){})},e.validateBankNumber=function(t){var n=a.defer();return r.checkBankNumber({number:t,userName:e.currentUser.userName},function(e){e.result?n.resolve(e):n.reject(e)}),n.promise}}]),mainApp.controller("userRegisterController",["$scope","userService","Notification","$state","$q",function(e,r,t,n,a){e.init=function(){e.newUser={userInfo:{}},e.submitted=!1},e.init(),e.registerUser=function(){e.submitted=!0,e.regForm.$valid&&(e.newUser.userInfo.parentId=e.currentUser.userName,r.register(e.newUser,function(e){t.success("Register successfully"),n.reload()}))},e.interacted=function(r){return e.submitted||r.$dirty},e.validateBankNumber=function(e){var t=a.defer();return r.checkBankNumber({number:e,userName:"*"},function(e){e.result?t.resolve(e):t.reject(e)}),t.promise},e.canIntroduce=function(){return null==e.currentUser||e.currentUser.userName.endsWith("A")||!e.currentUser.parentId}}]),mainApp.factory("userService",["$resource",function(e){return e(":path",{},{getCurrentUserContext:{method:"GET",params:{path:"api/user/getCurrentUserContext"}},getCurrentUserProfile:{method:"GET",params:{path:"api/user/getCurrent"}},getCurrentUserPinInfo:{method:"GET",params:{path:"api/user/getCurrentPin"}},getChildren:{method:"GET",params:{path:"api/user/getChildren",id:"id"},isArray:!0},register:{method:"POST",params:{path:"api/account/register",registerVM:"registerVM"},isArray:!0},updateCurrentUserProfile:{method:"POST",params:{path:"api/user/updateCurrent",userProfileVM:"userProfileVM"}},updateAvatar:{method:"POST",params:{path:"api/user/updateAvatar",avatar:"avatar"}},changePassword:{method:"POST",params:{path:"api/user/changePassword",model:"model"}},checkName:{method:"GET",params:{path:"api/user/checkName",name:"@name"}},checkBankNumber:{method:"GET",params:{path:"api/user/checkBankNumber",number:"@number",userName:"@userName"}},getCurrentAssociation:{method:"GET",params:{path:"api/user/getCurrentAssociation"},isArray:!0}})}]),mainApp.controller("userTreeController",["$scope","userService","_",function(e,r,t){e.getCurrentUserProfile=function(){return r.getCurrentUserProfile().$promise},e.getUserChildren=function(e){return r.getChildren({id:e}).$promise},e.init=function(){e.data=[],e.loadData()},e.loadData=function(){e.getCurrentUserProfile().then(function(r){e.data.push({id:r.id,title:r.displayName,name:r.userName})})},e.loadDataForSingleNode=function(r){r.isLoaded||(r.isLoaded=!0,e.getUserChildren(r.id).then(function(e){r.nodes=t.map(e,function(e){return{id:e.id,title:e.displayName,name:e.userName}})}))},e.init()}]);
=======
var mainApp=angular.module("mainApp",["ui.router","ui.bootstrap","authApp","ngResource","ui.tree","smart-table","underscore","angularFileUpload","ngImgCrop","ngMessages","angular-loading-bar","ui.validate","reCAPTCHA","commonApp","ui-notification"]);mainApp.config(["$stateProvider","$urlRouterProvider",function(e,t){t.otherwise("/"),e.state("home",{url:"/",templateUrl:"app/main/home/home.html"}).state("login",{url:"/login",templateUrl:"app/authentication/login/login.html",controller:"loginController"}).state("refresh",{url:"/refresh",templateUrl:"app/authentication/refresh/refresh.html",controller:"refreshController"}).state("pin",{url:"/pin",templateUrl:"app/main/pin/pin.html",controller:"pinController"}).state("rate",{url:"/rate",templateUrl:"app/main/pin/rate.html",controller:"rateController"}).state("user",{"abstract":!0,url:"/user",templateUrl:"app/main/user/user-info.html",controller:"userController"}).state("user.default",{url:"/",templateUrl:"app/main/user/user-info.default.html",controller:"userDefaultController"}).state("user.register",{url:"/register",templateUrl:"app/main/user/user-info.register.html",controller:"userRegisterController"}).state("user.tree",{url:"/tree",templateUrl:"app/main/user/user-info.tree.html",controller:"userTreeController"}).state("trade",{"abstract":!0,url:"/trade",templateUrl:"app/main/trade/trade-info.html",controller:"tradeInfoController"}).state("trade.statistic",{url:"/",templateUrl:"app/main/trade/trade-statistic.html",controller:"tradeStatisticController"}).state("trade.history",{url:"/history",templateUrl:"app/main/trade/trade-history.html",controller:"tradeHistoryController"}).state("trade.waitinggivers",{url:"/waitinggivers",templateUrl:"app/main/trade/trade-waitingGivers.html",controller:"tradeWaitingGiversController"}).state("trade.waitingreceivers",{url:"/waitingreceivers",templateUrl:"app/main/trade/trade-waitingReceivers.html",controller:"tradeWaitingReceiversController"}).state("currentTransaction",{url:"/trade/current",templateUrl:"app/main/transaction/current.html",controller:"transactionCurrentController"})}]),mainApp.config(["$httpProvider",function(e){e.interceptors.push("authInterceptorService")}]),mainApp.run(["authService",function(e){e.fillAuthData()}]),mainApp.config(["treeConfig",function(e){e.defaultCollapsed=!0}]),mainApp.config(["cfpLoadingBarProvider",function(e){e.includeSpinner=!1}]),mainApp.config(["reCAPTCHAProvider","recaptchaSettings",function(e,t){e.setPublicKey(t.publicKey),e.setOptions({theme:"red"})}]),mainApp.config(["NotificationProvider",function(e){e.setOptions({delay:1500,startTop:20,startRight:10,verticalSpacing:20,horizontalSpacing:20,positionX:"right",positionY:"bottom",closeOnClick:!0,maxCount:3})}]),mainApp.factory("AccountState",function(){var e={Default:0,WaitGive:1,Gave:2,WaitReceive:3,InGiveTransaction:11,InReceiveTransaction:12,NotGive:21,NotConfirm:22,ReportedNotTransfer:23};return e.display=function(t){switch(t){case e.Default:return"Default";case e.WaitGive:return"Wait Give";case e.Gave:return"Gave";case e.WaitReceive:return"Wait Receive";case e.InGiveTransaction:return"In Give Transaction";case e.InReceiveTransaction:return"In Receive Transaction";case e.NotGive:return"Not Give";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";default:return""}},e}),mainApp.factory("PriorityLevel",function(){var e={Default:0,Priority:1,High:2,ReceiveFail:3,Highest:10};return e.display=function(t){switch(t){case e.Default:return"Default";case e.Priority:return"Priority";case e.High:return"High";case e.ReceiveFail:return"Receive Fail";case e.Highest:return"Highest";default:return""}},e}),mainApp.factory("TransactionState",function(){var e={Begin:0,Transfered:1,Confirmed:2,NotTransfer:11,NotConfirm:12,ReportedNotTransfer:13};return e.display=function(t){switch(t){case e.Begin:return"Begin";case e.Transfered:return"Transfered";case e.Confirmed:return"Confirmed";case e.NotTransfer:return"Not Transfer";case e.NotConfirm:return"Not Confirm";case e.ReportedNotTransfer:return"Reported Not Transfer";default:return""}},e}),mainApp.filter("yesno",[function(){return function(e){return e?"Yes":"No"}}]),mainApp.controller("indexController",["$scope","$location","authService","userService",function(e,t,r,n){e.logOut=function(){r.logOut(),t.path("/home")},e.authentication=r.authentication,e.$on("user:authenticated",function(t,r){e.getUserContext()}),e.$on("user:updateAvatar",function(t,r){e.userContext.avatar.url=r}),e.getUserContext=function(){n.getCurrentUserContext(function(t){e.userContext=t})},e.getUserContext()}]),mainApp.controller("tradeCurrentController",["$scope","transactionService","tradeService","Notification","TransactionState",function(e,t,r,n,i){e.updateStatus=function(){return r.status({},function(t){console.log("2"),e.info=t}).$promise},e.getCurrentTransactions=function(){},e.init=function(){e.TransactionState=i,e.updateStatus().then(function(e){console.log("2")}),e.getCurrentTransactions()},e.init()}]),mainApp.controller("tradeHistoryController",["$scope","userService","transactionService","$q","Notification","$state",function(e,t,r,n,i,a){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),a.reload()},function(t){e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2)})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(e){console.log(e);var r=n.defer();return t.checkName({name:e},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise}}]),mainApp.controller("tradeInfoController",["$scope","tradeService","Notification","AccountState","PriorityLevel",function(e,t,r,n,i){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(e){info.isAllowGive=!0,r.success(e)})},e.queueReceive=function(){info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(e){info.isAllowReceive=!0,r.success(e)})},e.init=function(){r.success("Hello"),e.AccountState=n,e.PriorityLevel=i,e.updateStatus()},e.init()}]),mainApp.factory("tradeService",["$resource",function(e){return e(":path",{},{status:{method:"POST",params:{path:"api/trade/accountStatus"}},queueGive:{method:"POST",params:{path:"api/trade/queueGive"}},queueReceive:{method:"POST",params:{path:"api/trade/queueReceive"}}})}]),mainApp.controller("tradeStatisticController",["$scope","tradeService","Notification","AccountState","PriorityLevel",function(e,t,r,n,i){e.updateStatus=function(){t.status({},function(t){e.info=t})},e.queueGive=function(){info.isAllowGive=!1,t.queueGive({},function(t){r.success("Queue give successful!"),e.updateStatus()},function(e){info.isAllowGive=!0,r.success(e)})},e.queueReceive=function(){info.isAllowReceive=!1,t.queueReceive({},function(t){r.success("Queue receive successful!"),e.updateStatus()},function(e){info.isAllowReceive=!0,r.success(e)})},e.init=function(){r.success("Hello"),e.accountStates=n,e.priorityLevels=i,e.updateStatus()},e.init()}]),mainApp.controller("tradeWaitingGiversController",["$scope","userService","tradeService","$q","Notification","$state",function(e,t,r,n,i,a){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),a.reload()},function(t){e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2)})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(e){console.log(e);var r=n.defer();return t.checkName({name:e},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise}}]),mainApp.controller("tradeWaitingReceiversController",["$scope","userService","tradeService","$q","Notification","$state",function(e,t,r,n,i,a){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),a.reload()},function(t){e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2)})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(e){console.log(e);var r=n.defer();return t.checkName({name:e},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise}}]),mainApp.controller("pinController",["$scope","userService","pinService","$q","Notification","$state",function(e,t,r,n,i,a){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),a.reload()},function(t){e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2)})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(e){console.log(e);var r=n.defer();return t.checkName({name:e},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise}}]),mainApp.factory("pinService",["$resource",function(e){return e(":path",{},{transfer:{method:"POST",params:{path:"api/pin/transfer",transactionVM:"transactionVM"}},getCurrentUserHistory:{method:"GET",params:{path:"api/pin/getAll"},isArray:!0}})}]),mainApp.controller("rateController",["$scope","userService","$q","Notification","$state","$http",function(e,t,r,n,i,a){e.init=function(){$.getJSON("https://bitpay.com/api/rates",function(t){e.rates=t;for(var r=0;r<t.length;r++)"USD"==t[r].code&&(e.currRate=t[r].rate)})},e.init(),e.validateUser=function(e){console.log(e);var n=r.defer();return t.checkName({name:e},function(e){e.result?n.resolve(e):n.reject(e)}),n.promise}}]),mainApp.controller("transactionCurrentController",["$scope","_","transactionService","tradeService","Notification","AccountState","TransactionState",function(e,t,r,n,i,a,o){function s(r){t.each(r,function(e){e.isBegin=e.state==o.Begin,e.isAllowConfirmGave=e.state==o.Begin,e.isAllowConfirmReceived=e.state==o.Transfered,console.log(e)}),e.transactions=r}e.updateStatus=function(){return n.status({},function(t){e.info=t}).$promise},e.getCurrentTransactions=function(){e.info.state==a.InGiveTransaction?(r.giveRequested({},function(e){s(e)}),e.accountDisplayTemplate=e.receiverInfoTemplateUrl):e.info.state==a.InReceiveTransaction&&(r.receiveRequested({},function(e){s(e)}),e.accountDisplayTemplate=e.giverInfoTemplateUrl)},e.moneyTransfered=function(t){r.moneyTransfered(t,function(r){i.success("Money Transfered");var n=e.transactions.indexOf(t);-1!==n&&(e.transactions[n]=r)})},e.moneyReceived=function(t){r.moneyReceived(t,function(r){i.success("Money Received");var n=e.transactions.indexOf(t);-1!==n&&(e.transactions[n]=r)})},e.reportNotTransfer=function(t){r.reportNotTransfer(t,function(r){i.success("Transaction Reported");var n=e.transactions.indexOf(t);-1!==n&&(e.transactions[n]=r)})},e.init=function(){e.giverInfoTemplateUrl="app/main/transaction/info-giver.html",e.receiverInfoTemplateUrl="app/main/transaction/info-receiver.html",e.TransactionState=o,e.AccountState=a,e.updateStatus().then(function(t){e.getCurrentTransactions()})},e.init()}]),mainApp.controller("transactionHistoryController",["$scope","userService","transactionService","$q","Notification","$state",function(e,t,r,n,i,a){e.getCurrentUserPinInfo=function(){return t.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return r.getCurrentUserHistory().$promise},e.init=function(){e.failed=0,e.submitted=!1,e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(t){e.currentPinBalance=t}),e.getCurrentUserPinTransactionHistory().then(function(t){e.transactionHistories=t}),e.transaction={}},e.init(),e.transferPIN=function(){e.submitted=!0,e.frmTransfer.$valid&&r.transfer(e.transaction,function(e){i.success("Transaction has been completed successfully"),a.reload()},function(t){e.failed=1,"invalid_captcha"==t.data.message&&(e.failed=2)})},e.interacted=function(t){return e.submitted||t.$dirty},e.validateUser=function(e){console.log(e);var r=n.defer();return t.checkName({name:e},function(e){e.result?r.resolve(e):r.reject(e)}),r.promise}}]),mainApp.factory("transactionService",["$resource",function(e){return e(":path",{},{history:{method:"POST",params:{path:"api/transaction/history"},isArray:!0},giveRequested:{method:"POST",params:{path:"api/transaction/giveRequested"},isArray:!0},receiveRequested:{method:"POST",params:{path:"api/transaction/receiveRequested"},isArray:!0},moneyTransfered:{method:"POST",params:{path:"api/transaction/moneyTransfered"}},moneyReceived:{method:"POST",params:{path:"api/transaction/moneyReceived"}},reportNotTransfer:{method:"POST",params:{path:"api/transaction/reportNotTransfer"}}})}]),mainApp.controller("dlgChangeAvatarController",["$scope","userService","FileUploader","_","localStorageService","$uibModalInstance","Notification",function(e,t,r,n,i,a,o){e.uploadImage=function(){var e=n.last(s.queue);e.upload()};var s=e.uploader=new r({autoUpload:!1,url:"api/user/updateAvatar"});s.filters.push({name:"size",fn:function(e){return e.size<=5242880}}),s.filters.push({name:"image",fn:function(e){return s.hasHTML5?/\/(png|jpeg|jpg)$/.test(e.file.type):!0}});var c=i.get("authorizationData");c&&(s.headers.Authorization="Bearer "+c.token),s.onAfterAddingFile=function(t){e.cropped={image:""};var r=new FileReader;r.onload=function(t){e.$apply(function(){e.image=t.target.result})},r.readAsDataURL(t._file)},s.onBeforeUploadItem=function(t){var r=u(e.cropped.image);t._file=r};var u=function(e){var t;t=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):decodeURI(e.split(",")[1]);for(var r=e.split(",")[0].split(":")[1].split(";")[0],n=[],i=0;i<t.length;i++)n.push(t.charCodeAt(i));return new Blob([new Uint8Array(n)],{type:r})};s.onSuccessItem=function(t,r,n,i){console.log(r),e.ok(r)},e.ok=function(e){o.success("Avatar has been updated"),a.close(e)},e.cancel=function(){a.dismiss()}}]),mainApp.controller("dlgChangePasswordController",["$scope","userService","$uibModalInstance","Notification",function(e,t,r,n){e.interacted=function(t){return e.submitted||t.$dirty},e.init=function(){e.failed=!1,e.submitted=!1,e.model={}},e.init(),e.changePassword=function(e){return t.changePassword(e).$promise},e.submit=function(){e.submitted=!0,e.frmPassword.$valid&&e.changePassword(e.model).then(function(t){t.result?e.ok():(e.failed=!0,n.error("Failed to update your password"))})},e.ok=function(e){n.success("Password has been changed successfully"),r.close(e)},e.cancel=function(){r.dismiss()}}]),mainApp.controller("userController",["$scope","userService","imageService","$uibModal",function(e,t,r,n){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.init=function(){e.currentUser=e.currentUser||null,null==e.currentUser&&e.getCurrentUserProfile().then(function(t){e.currentUser=t})},e.init(),e.updateAvatar=function(){var t=n.open({animation:!1,templateUrl:"app/main/user/dlg-change-avatar.html",controller:"dlgChangeAvatarController",size:"lg",windowClass:"portraitDialog"});t.result.then(function(t){e.currentUser=t,e.currentUser.avatar.url=e.currentUser.avatar.url+"?"+(new Date).getTime(),e.$emit("user:updateAvatar",e.currentUser.avatar.url)},function(){})}}]),mainApp.controller("userDefaultController",["$scope","userService","$uibModal","Notification",function(e,t,r,n){e.updateCurrentUserProfile=function(e){return t.updateCurrentUserProfile(e).$promise},e.updateProfile=function(){e.userForm.$valid&&e.updateCurrentUserProfile(e.currentUser).then(function(e){n.success("Update successfully")})},e.changePassword=function(){var e=r.open({animation:!1,templateUrl:"app/main/user/dlg-change-password.html",controller:"dlgChangePasswordController",size:"md",windowClass:"passswordDialog"});e.result.then(function(e){},function(){})}}]),mainApp.controller("userRegisterController",["$scope","userService","Notification","$state",function(e,t,r,n){e.init=function(){e.newUser={userInfo:{}},e.submitted=!1},e.init(),e.registerUser=function(){e.submitted=!0,e.regForm.$valid&&(e.newUser.userInfo.parentId=e.currentUser.userName,t.register(e.newUser,function(e){r.success("Register successfully"),n.reload()}))},e.interacted=function(t){return e.submitted||t.$dirty}}]),mainApp.factory("userService",["$resource",function(e){return e(":path",{},{getCurrentUserContext:{method:"GET",params:{path:"api/user/getCurrentUserContext"}},getCurrentUserProfile:{method:"GET",params:{path:"api/user/getCurrent"}},getCurrentUserPinInfo:{method:"GET",params:{path:"api/user/getCurrentPin"}},getChildren:{method:"GET",params:{path:"api/user/getChildren",id:"id"},isArray:!0},register:{method:"POST",params:{path:"api/account/register",registerVM:"registerVM"},isArray:!0},updateCurrentUserProfile:{method:"POST",params:{path:"api/user/updateCurrent",userProfileVM:"userProfileVM"}},updateAvatar:{method:"POST",params:{path:"api/user/updateAvatar",avatar:"avatar"}},changePassword:{method:"POST",params:{path:"api/user/changePassword",model:"model"}},checkName:{method:"GET",params:{path:"api/user/checkName",name:"@name"}}})}]),mainApp.controller("userTreeController",["$scope","userService","_",function(e,t,r){e.getCurrentUserProfile=function(){return t.getCurrentUserProfile().$promise},e.getUserChildren=function(e){return t.getChildren({id:e}).$promise},e.init=function(){e.data=[],e.loadData()},e.loadData=function(){e.getCurrentUserProfile().then(function(t){e.data.push({id:t.id,title:t.displayName,name:t.userName})})},e.loadDataForSingleNode=function(t){t.isLoaded||(t.isLoaded=!0,e.getUserChildren(t.id).then(function(e){t.nodes=r.map(e,function(e){return{id:e.id,title:e.displayName,name:e.userName}})}))},e.init()}]);
>>>>>>> 2ecd1b6a149f29682fb10f47e017d022e17562e0
