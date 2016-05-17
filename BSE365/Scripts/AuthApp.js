var authApp=angular.module("authApp",["LocalStorageModule","commonApp"]);authApp.controller("loginController",["$scope","$location","authService","ngAuthSettings",function(e,r,t,o){e.loginData={userName:"",password:"",useRefreshTokens:!0},e.message="",e.login=function(){console.log(123),t.login(e.loginData).then(function(e){r.path("/")},function(r){e.message=r.error_description})},e.authExternalProvider=function(r){var t=location.protocol+"//"+location.host+"/authcomplete.html",n=o.apiServiceBaseUri+"api/Account/ExternalLogin?provider="+r+"&response_type=token&client_id="+o.clientId+"&redirect_uri="+t;window.$windowScope=e;window.open(n,"Authenticate Account","location=0,status=0,width=600,height=750")},e.authCompletedCB=function(o){e.$apply(function(){if("False"==o.haslocalaccount)t.logOut(),t.externalAuthData={provider:o.provider,userName:o.external_user_name,externalAccessToken:o.external_access_token},r.path("/associate");else{var n={provider:o.provider,externalAccessToken:o.external_access_token};t.obtainAccessToken(n).then(function(e){r.path("/")},function(r){e.message=r.error_description})}})}}]),authApp.factory("authInterceptorService",["$q","$injector","$location","localStorageService",function(e,r,t,o){var n={},s=function(e){e.headers=e.headers||{};var r=o.get("authorizationData");return r&&(e.headers.Authorization="Bearer "+r.token),e},a=function(n){if(401===n.status){var s=r.get("authService"),a=o.get("authorizationData");if(a&&a.useRefreshTokens)return t.path("/refresh"),e.reject(n);s.logOut(),t.path("/login")}return e.reject(n)};return n.request=s,n.responseError=a,n}]),authApp.factory("authService",["$http","$q","localStorageService","ngAuthSettings",function(e,r,t,o){var n=o.apiServiceBaseUri,s={},a={isAuth:!1,userName:"",useRefreshTokens:!1},i={provider:"",userName:"",externalAccessToken:""},u=function(r){return h(),e.post(n+"api/account/register",r).then(function(e){return e})},c=function(s){var i="grant_type=password&username="+s.userName+"&password="+s.password;s.useRefreshTokens&&(i=i+"&client_id="+o.clientId);var u=r.defer();return e.post(n+"token",i,{headers:{"Content-Type":"application/x-www-form-urlencoded"}}).success(function(e){s.useRefreshTokens?t.set("authorizationData",{token:e.access_token,userName:s.userName,refreshToken:e.refresh_token,useRefreshTokens:!0}):t.set("authorizationData",{token:e.access_token,userName:s.userName,refreshToken:"",useRefreshTokens:!1}),a.isAuth=!0,a.userName=s.userName,a.useRefreshTokens=s.useRefreshTokens,u.resolve(e)}).error(function(e,r){h(),u.reject(e)}),u.promise},h=function(){t.remove("authorizationData"),a.isAuth=!1,a.userName="",a.useRefreshTokens=!1},f=function(){var e=t.get("authorizationData");e&&(a.isAuth=!0,a.userName=e.userName,a.useRefreshTokens=e.useRefreshTokens)},l=function(){var s=r.defer(),a=t.get("authorizationData");if(a&&a.useRefreshTokens){var i="grant_type=refresh_token&refresh_token="+a.refreshToken+"&client_id="+o.clientId;t.remove("authorizationData"),e.post(n+"token",i,{headers:{"Content-Type":"application/x-www-form-urlencoded"}}).success(function(e){t.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:e.refresh_token,useRefreshTokens:!0}),s.resolve(e)}).error(function(e,r){h(),s.reject(e)})}return s.promise},p=function(o){var s=r.defer();return e.get(n+"api/account/ObtainLocalAccessToken",{params:{provider:o.provider,externalAccessToken:o.externalAccessToken}}).success(function(e){t.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:"",useRefreshTokens:!1}),a.isAuth=!0,a.userName=e.userName,a.useRefreshTokens=!1,s.resolve(e)}).error(function(e,r){h(),s.reject(e)}),s.promise},k=function(o){var s=r.defer();return e.post(n+"api/account/registerexternal",o).success(function(e){t.set("authorizationData",{token:e.access_token,userName:e.userName,refreshToken:"",useRefreshTokens:!1}),a.isAuth=!0,a.userName=e.userName,a.useRefreshTokens=!1,s.resolve(e)}).error(function(e,r){h(),s.reject(e)}),s.promise};return s.saveRegistration=u,s.login=c,s.logOut=h,s.fillAuthData=f,s.authentication=a,s.refreshToken=l,s.obtainAccessToken=p,s.externalAuthData=i,s.registerExternal=k,s}]);