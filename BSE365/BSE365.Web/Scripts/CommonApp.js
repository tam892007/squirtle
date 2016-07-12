var commonApp=angular.module("commonApp",["ngResource","ui.router","ui.bootstrap"]),serviceBase="/";commonApp.constant("ngAuthSettings",{apiServiceBaseUri:serviceBase,clientId:"SHARE"}),commonApp.constant("recaptchaSettings",{publicKey:"6LcFKCETAAAAAN6JPAwzot2fExNpZUumgKXj Jugq"}),commonApp.constant("filterSetting",{pagination:{template:"template/smart-table/pagination.html",itemsByPage:30,displayedPages:10},search:{delay:400,inputEvent:"input"},select:{mode:"single",selectedClass:"st-selected"},sort:{ascentClass:"st-sort-ascent",descentClass:"st-sort-descent",descendingFirst:!1,skipNatural:!1,delay:300},pipe:{delay:100}}),angular.module("smart-table").directive("stPaginationScroll",["$timeout",function(e){return{require:"stTable",link:function(n,t,i,a){var r=100,c=a.tableState().pagination,o=50,l=400,u=function(){a.slice(c.start+r,r)},s=null,g=9999999,d=angular.element(t.parent());d.bind("scroll",function(){var n=d[0].scrollHeight-(d[0].clientHeight+d[0].scrollTop);o>n&&0>n-g&&(null!==s&&e.cancel(s),s=e(function(){u(),s=null},l)),g=n})}}}]).directive("pageSelect",function(){return{restrict:"E",template:'<input type="text" class="select-page" ng-model="inputPage" ng-model-options="{ updateOn: \'default blur\', debounce: { \'default\': 500, \'blur\': 0 } }" ng-change="selectPage(inputPage)">',link:function(e,n,t){e.$watch("currentPage",function(n){e.inputPage=n})}}});var underscore=angular.module("underscore",[]);underscore.factory("_",function(){return window._}),commonApp.directive("ngConfirmClick",[function(){return{restrict:"A",link:function(e,n,t){n.bind("click",function(){var n=t.ngConfirmMessage;n&&confirm(n)&&e.$apply(t.ngConfirmClick)})}}}]),commonApp.directive("ngReallyClick",["$uibModal",function(e){var n=["$scope","$uibModalInstance",function(e,n){e.ok=function(){n.close()},e.cancel=function(){n.dismiss("cancel")}}];return{restrict:"A",scope:{ngReallyClick:"&"},link:function(t,i,a){i.bind("click",function(){var i=a.ngReallyMessage||"Are you sure ?",r='<div class="modal-body">'+i+"</div>";r+='<div class="modal-footer"><button class="btn btn-primary" ng-click="ok()">OK</button><button class="btn btn-warning" ng-click="cancel()" >Close</button></div>';var c=e.open({template:r,controller:n});c.result.then(function(){t.ngReallyClick()},function(){})})}}}]),commonApp.directive("ngEnter",[function(){return function(e,n,t){n.bind("keydown keypress",function(n){13===n.which&&(e.$apply(function(){e.$eval(t.ngEnter)}),n.preventDefault())})}}]),commonApp.directive("httpSrc",["$http",function(e){return{link:function(n,t,i){function a(){n.objectURL&&URL.revokeObjectURL(n.objectURL)}n.$watch("objectURL",function(e){t.attr("src",e)}),n.$on("$destroy",function(){a()}),i.$observe("httpSrc",function(t){a(),t&&0===t.indexOf("data:")?n.objectURL=t:t&&e.get(t,{responseType:"arraybuffer"}).then(function(e){var t=new Blob([e.data],{type:e.headers("Content-Type")});n.objectURL=URL.createObjectURL(t)})})}}}]),commonApp.directive("syncFocusWith",["$timeout",function(e){return{restrict:"A",scope:{focusValue:"=syncFocusWith"},link:function(e,n,t){e.$watch("focusValue",function(e,t){1==e?n[0].focus():n[0].blur()})}}}]),commonApp.factory("imageService",["$resource",function(e){return e(":path",{},{getUserPic:{method:"GET",params:{path:"api/image/getUserPicture",id:"@id"}}})}]),angular.module("localization",["LocalStorageModule"]).provider("localize",function(){this.languages=["en-US","vi-VN"],this.defaultLanguage="en-US",this.ext="js",this.baseUrl="i18n/";var e=this;this.$get=["$http","$rootScope","$window","$filter","localStorageService",function(n,t,i,a,r){var c={language:"",dictionary:[],url:void 0,resourceFileLoaded:!1,successCallback:function(e){c.dictionary=e,c.resourceFileLoaded=!0,t.$broadcast("localizeResourcesUpdated")},setLanguage:function(e){c.language=this.fallbackLanguage(e),r.cookie.set("language",c.language),c.initLocalizedResources()},fallbackLanguage:function(n){return n=String(n),e.languages.indexOf(n)>-1?n:(n=n.split("-")[0],e.languages.indexOf(n)>-1?n:e.defaultLanguage)},isCurrentLanguage:function(e){var n=this.fallbackLanguage(e);return n==c.language},setUrl:function(e){c.url=e,c.initLocalizedResources()},buildImgUrl:function(e){return n({method:"GET",url:e,cache:!1})},buildUrl:function(n){if(!c.language){var t=r.cookie.get("language");if(t)c.language=t;else{var a,o;a=i.navigator&&i.navigator.userAgent&&(o=i.navigator.userAgent.match(/android.*\W(\w\w)-(\w\w)\W/i))?o[1]:i.navigator.userLanguage||i.navigator.language,c.language=this.fallbackLanguage(a)}}return n+"resources-locale_"+c.language+"."+e.ext},initLocalizedResources:function(){var t=c.url||c.buildUrl(e.baseUrl);n({method:"GET",url:t,cache:!1}).success(c.successCallback).error(function(){var t=e.baseUrl+"resources-locale_default."+e.ext;n({method:"GET",url:t,cache:!1}).success(c.successCallback)})},getLocalizedString:function(e){var n="";if(c.resourceFileLoaded){var t=a("filter")(c.dictionary,function(n){return n.key===e})[0];t||console.log(e),n=t.value?t.value:e}return n}};return c.initLocalizedResources(),c}]}).filter("i18n",["localize",function(e){return function(n){return e.getLocalizedString(n)}}]).directive("i18n",["localize",function(e){var n={restrict:"EAC",updateText:function(n,t,i){var a=t.split("|");if(a.length>=1){var r=e.getLocalizedString(a[0]);if(null!==r&&void 0!==r&&""!==r){if(a.length>1)for(var c=1;c<a.length;c++){var o="{"+(c-1)+"}";r=r.replace(o,a[c])}n[i?"html":"text"](r)}}},link:function(e,t,i){e.$on("localizeResourcesUpdated",function(){n.updateText(t,i.i18n,angular.isDefined(i.i18nHtml))}),i.$observe("i18n",function(e){n.updateText(t,i.i18n,angular.isDefined(i.i18nHtml))})}};return n}]).directive("i18nAttr",["localize",function(e){var n={restrict:"EAC",updateText:function(n,t){var i=t.split("|"),a=e.getLocalizedString(i[0]);if(null!==a&&void 0!==a&&""!==a){if(i.length>2)for(var r=2;r<i.length;r++){var c="{"+(r-2)+"}";a=a.replace(c,i[r])}n.attr(i[1],a)}},link:function(e,t,i){e.$on("localizeResourcesUpdated",function(){n.updateText(t,i.i18nAttr)}),i.$observe("i18nAttr",function(e){n.updateText(t,e)})}};return n}]).directive("i18nImgSrc",["localize",function(e){var n={restrict:"A",link:function(n,t,i){var a=i.i18nImgSrc,r=provider.baseUrl+"/images/"+e.language+"/",c=r+a;e.buildImgUrl(c).success(function(){t[0].src=c}).error(function(){t[0].src=provider.baseUrl+"/images/default/"+a})}};return n}]);