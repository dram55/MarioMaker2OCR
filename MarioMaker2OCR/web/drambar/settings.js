(function(){
   'use strict';

   var app = angular.module('dramBarApp');
   app.constant('dramBarSettings', {

      // Which Components to see?
      isTimerVisible:true, 
      isDeathCountVisible:true,
      isLevelNameVisible:true,
      isLevelAuthorVisible:true,
      isLevelCodeVisible:true,


      // How do they look?
      slideFromDirection:'left', //left|right|top|bottom
      levelCodeColor: '#ff49ad',
      levelNameColor: '#f5f5f5',
      levelAuthorColor: '#f5f5f5',
      fontSize: '100%',
      

      //Other options
      playTimerWarning:false,
      playTimerWarningAt:15, //minutes
   });
})();