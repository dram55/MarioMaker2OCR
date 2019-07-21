(function(){
   'use strict';

   var app = angular.module('dramBarApp');
   app.constant('dramBarSettings', {
      isTimerVisible:true, 
      isDeathCountVisible:true,
      slideFromDirection:'left', //left|right|top|bottom
   });

})();