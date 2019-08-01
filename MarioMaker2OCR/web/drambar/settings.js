(function(){
   'use strict';

   var app = angular.module('dramBarApp');
   app.constant('dramBarSettings', {
      isTimerVisible:true, 
      isDeathCountVisible:true,
      isLevelNameVisible:true,
      isLevelAuthorVisible:true,
      isLevelCodeVisible:true,
      slideFromDirection:'left', //left|right|top|bottom
      playTimerWarning:false,
      playTimerWarningAt:15, //minutes
   });

})();