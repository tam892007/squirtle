var mainApp=angular.module("mainApp",["ui.router","authApp","ngResource","ui.tree","smart-table","underscore"]);mainApp.config(["$stateProvider","$urlRouterProvider",function(e,r){r.otherwise("/"),e.state("home",{url:"/",templateUrl:"app/main/home/home.html"}).state("login",{url:"/login",templateUrl:"app/authentication/login/login.html",controller:"loginController"}).state("pin",{url:"/pin",templateUrl:"app/main/pin/pin.html",controller:"pinController"}).state("user",{"abstract":!0,url:"/user",templateUrl:"app/main/user/user-info.html",controller:"userController"}).state("user.default",{url:"/",templateUrl:"app/main/user/user-info.default.html",controller:"userDefaultController"}).state("user.register",{url:"/register",templateUrl:"app/main/user/user-info.register.html",controller:"userRegisterController"}).state("user.tree",{url:"/tree",templateUrl:"app/main/user/user-info.tree.html",controller:"userTreeController"})}]),mainApp.config(["$httpProvider",function(e){e.interceptors.push("authInterceptorService")}]),mainApp.run(["authService",function(e){e.fillAuthData()}]),mainApp.config(["treeConfig",function(e){e.defaultCollapsed=!0}]),mainApp.controller("pinController",["$scope","userService","pinService",function(e,r,t){e.getCurrentUserPinInfo=function(){return r.getCurrentUserPinInfo().$promise},e.getCurrentUserPinTransactionHistory=function(){return t.getCurrentUserHistory().$promise},e.init=function(){e.currentPinBalance={},e.transactionHistories=[],e.getCurrentUserPinInfo().then(function(r){e.currentPinBalance=r}),e.getCurrentUserPinTransactionHistory().then(function(r){e.transactionHistories=r}),e.transaction={}},e.init(),e.transferPIN=function(){t.transfer(e.transaction,function(r){e.init()})}}]),mainApp.factory("pinService",["$resource",function(e){return e(":path",{},{transfer:{method:"POST",params:{path:"api/pin/transfer",transactionVM:"transactionVM"}},getCurrentUserHistory:{method:"GET",params:{path:"api/pin/getAll"},isArray:!0}})}]),mainApp.controller("userController",["$scope","userService",function(e,r){e.getCurrentUserProfile=function(){return r.getCurrentUserProfile().$promise},e.init=function(){e.currentUser=e.currentUser||null,null==e.currentUser&&e.getCurrentUserProfile().then(function(r){e.currentUser=r})},e.init()}]),mainApp.controller("userDefaultController",["$scope","userService",function(e,r){}]),mainApp.controller("userRegisterController",["$scope","userService",function(e,r){e.init=function(){e.newUser={userInfo:{}}},e.init(),e.registerUser=function(){e.newUser.userInfo.parentId=e.currentUser.id,r.register(e.newUser,function(e){})}}]),mainApp.factory("userService",["$resource",function(e){return e(":path",{},{getCurrentUserProfile:{method:"GET",params:{path:"api/user/getCurrent"}},getCurrentUserPinInfo:{method:"GET",params:{path:"api/user/getCurrentPin"}},getChildren:{method:"GET",params:{path:"api/user/getChildren",id:"id"},isArray:!0},register:{method:"POST",params:{path:"api/account/register",registerVM:"registerVM"}}})}]),mainApp.controller("userTreeController",["$scope","userService","_",function(e,r,t){e.getCurrentUserProfile=function(){return r.getCurrentUserProfile().$promise},e.getUserChildren=function(e){return r.getChildren({id:e}).$promise},e.init=function(){e.data=[],e.loadData()},e.loadData=function(){e.getCurrentUserProfile().then(function(r){e.data.push({id:r.id,title:r.displayName})})},e.loadDataForSingleNode=function(r){r.isLoaded||(r.isLoaded=!0,e.getUserChildren(r.id).then(function(e){r.nodes=t.map(e,function(e){return{id:e.id,title:e.displayName}})}))},e.init()}]);