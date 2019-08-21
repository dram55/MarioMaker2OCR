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

      // innerFadeAuthorAndName: 8000, // not yet supported.

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

     // SMM Colors
	  //
	  //rgba(61, 80, 174, 1)   // Blue
	  //rgba(20, 33, 112, 1)   // Dark Blue
	  //rgba(38, 174, 160, 1)  // Teal
	  //rgba(227, 143, 21, 1)  // Dark Yellow
	  //rgba(220, 70, 116, 1)  // Pink
	  
	  //rgba(250, 205, 0, 1)   // Yellow //#facd00
	  //rgba(92, 28, 28, 1)    // Dark Brown //#5c1c1c

     // 4th argument in rbga(0,0,0,X) is opacity.
	  // 0 = not visible.
	  // 1 = opaque
   });
})();