var commonApp=angular.module("commonApp",[]),serviceBase="http://localhost:2736/";commonApp.constant("ngAuthSettings",{apiServiceBaseUri:serviceBase,clientId:""});var underscore=angular.module("underscore",[]);underscore.factory("_",function(){return window._});