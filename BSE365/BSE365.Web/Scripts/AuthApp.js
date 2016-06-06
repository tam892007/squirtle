var authApp=angular.module("authApp",["LocalStorageModule","commonApp"]);authApp.controller("loginController",["$scope","$location","authService","ngAuthSettings",function(e,t,r,n){e.forgotPassword=function(){e.init(),e.state=2},e.requestNewPassword=function(){r.forgetPassword(e.forgotUserName).then(function(t){e.message="Successfully. Please check your email."},function(t){e.message=t.error_description})},e.init=function(){e.message="",e.state=1,e.loginData={userName:"",password:"",useRefreshTokens:!0},e.forgotUserName="",e.submitted=!1},e.init(),e.login=function(){e.submitted=!0,e.frmLogin.$valid&&r.login(e.loginData).then(function(e){t.path("/")},function(t){e.message=t.error_description})},e.authExternalProvider=function(t){var r=location.protocol+"//"+location.host+"/authcomplete.html",o=n.apiServiceBaseUri+"api/Account/ExternalLogin?provider="+t+"&response_type=token&client_id="+n.clientId+"&redirect_uri="+r;window.$windowScope=e;window.open(o,"Authenticate Account","location=0,status=0,width=600,height=750")},e.authCompletedCB=function(n){e.$apply(function(){if("False"==n.haslocalaccount)r.logOut(),r.externalAuthData={provider:n.provider,userName:n.external_user_name,externalAccessToken:n.external_access_token},t.path("/associate");else{var o={provider:n.provider,externalAccessToken:n.external_access_token};r.obtainAccessToken(o).then(function(e){t.path("/")},function(t){e.message=t.error_description})}})},e.interacted=function(t){return e.submitted||t.$dirty}}]),authApp.controller("refreshController",["$scope","$location","authService",function(e,t,r){e.authentication=r.authentication,e.tokenRefreshed=!1,e.tokenResponse=null,e.refreshToken=function(){r.refreshToken().then(function(t){e.tokenRefreshed=!0,e.tokenResponse=t},function(e){t.path("/login")})},e.refreshToken()}]),authApp.factory("authInterceptorService",["$q","$injector","$location","localStorageService",function(e,t,r,n){var o={},s=function(e){if(e.skipInterceptor)return e;e.headers=e.headers||{};var t=n.get("authorizationData");return t&&(e.headers.Authorization="Bearer "+t.token),e},a=function(o){if(401===o.status){var s=t.get("authService"),a=n.get("authorizationData");if(a&&a.useRefreshTokens){var i=t.get("$state");return s.refreshToken().then(function(e){i.reload()},function(e){r.path("/login")}),e.reject(o)}s.logOut(),r.path("/login")}return e.reject(o)};return o.request=s,o.responseError=a,o}]),authApp.factory("authService",["$http","$q","localStorageService","ngAuthSettings","$rootScope",function(e,t,r,n,o){var s=n.apiServiceBaseUri,a={},i={isAuth:!1,userName:"",useRefreshTokens:!1},u={provider:"",userName:"",externalAccessToken:""},c=function(t){return h(),e.post(s+"api/account/register",t).then(function(e){return e})},f=function(a){var u="grant_type=password&username="+a.userName+"&password="+a.password;a.useRefreshTokens&&(u=u+"&client_id="+n.clientId);var c=t.defer();return e.post(s+"token",u,{headers:{"Content-Type":"application/x-www-form-urlencoded"}}).success(function(e){a.useRefreshTokens?r.set("authorizationData",{token:e.access_token,userName:a.userName,refreshToken:e.refresh_token,useRefreshTokens:!0}):r.set("authorizationData",{token:e.access_token,userName:a.userName,refreshToken:"",useRefreshTokens:!1}),i.isAuth=!0,i.userName=a.userName,i.useRefreshTokens=a.useRefreshTokens,o.$broadcast("user:authenticated"),c.resolve(e)}).error(function(e,t){c.reject(e)}),c.promise},h=function(){r.remove("authorizationData"),i.isAuth=!1,i.userName="",i.useRefreshTokens=!1},l=function(){var e=r.get("authorizationData");e&&(i.isAuth=!0,i.userName=e.userName,i.useRefreshTokens=e.useRefreshTokens)},p=function(){var i=t.defer(),u=r.get("authorizationData");if(u&&!a.tokenRefreshing&&u.useRefreshTokens){a.tokenRefreshing=!0;var c="grant_type=refresh_token&refresh_token="+u.refreshToken+"&client_id="+n.clientId;e.post(s+"token",c,{headers:{"Content-Type":"application/x-www-form-urlencoded"}}).success(function(e){r.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:e.refresh_token,useRefreshTokens:!0}),o.$broadcast("user:authenticated"),i.resolve(e)}).error(function(e,t){h(),i.reject(e)})["finally"](function(){a.tokenRefreshing=!1})}return i.promise},d=function(n){var o=t.defer();return e.get(s+"api/account/ObtainLocalAccessToken",{params:{provider:n.provider,externalAccessToken:n.externalAccessToken}}).success(function(e){r.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:"",useRefreshTokens:!1}),i.isAuth=!0,i.userName=e.userName,i.useRefreshTokens=!1,o.resolve(e)}).error(function(e,t){h(),o.reject(e)}),o.promise},k=function(n){var o=t.defer();return e.post(s+"api/account/registerexternal",n).success(function(e){r.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:"",useRefreshTokens:!1}),i.isAuth=!0,i.userName=e.userName,i.useRefreshTokens=!1,o.resolve(e)}).error(function(e,t){h(),o.reject(e)}),o.promise},m=function(r){var n=t.defer();return e.get(s+"api/account/forgotpassword",{params:{userName:r}}).success(function(e){n.resolve(e)}).error(function(e,t){n.reject(e)}),n.promise},g=function(r){var n=t.defer();return e.post(s+"api/account/resetpassword",r).success(function(e){n.resolve(e)}).error(function(e,t){n.reject(e)}),n.promise};return a.saveRegistration=c,a.login=f,a.logOut=h,a.fillAuthData=l,a.authentication=i,a.refreshToken=p,a.obtainAccessToken=d,a.externalAuthData=u,a.registerExternal=k,a.forgetPassword=m,a.resetPassword=g,a}]),authApp.controller("resetPasswordController",["$scope","$location","authService","$state",function(e,t,r,n){e.init=function(){var r=t.search();e.message="",e.resetData={userName:r.name,code:r.code},e.submitted=!1,e.step=1},e.init(),e.reset=function(){e.submitted=!0,e.frmReset.$valid&&r.resetPassword(e.resetData).then(function(t){e.message="Your password has been updated.",e.step=2},function(t){e.step=2,e.message="The reset link is out of date. Please request a new one."})},e.backToLogin=function(){n.go("login")},e.interacted=function(t){return e.submitted||t.$dirty}}]);