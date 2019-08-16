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

      border: 'none', // border: '1px solid black',

      levelCodeColor: '#ff49ad',
      levelNameColor: '#f5f5f5',
      levelAuthorColor: '#f5f5f5',
      timerFontColor: '#f5f5f5',
      deathCountFontColor: '#f5f5f5',
      timerColorOnClear: '#10ff00',

      fontSize: '100%',
      mainFont: 'Segoe UI',
      levelCodeFont: 'Consolas',

      backgroundColor: 'rgba(0, 0, 0, 0.77)',



      //Other options
      playTimerWarning:false,
      playTimerWarningAt:15, //minutes
   });
})();