(function(){
   'use strict';

   var app = angular.module('dramBarApp');
   app.constant('dramBarSettings', {

      // Which Components to see?
      isTimerVisible:true, 
      isDeathCountVisible:true,
      isLevelNameVisible:true,
      isLevelAuthorVisible:false,
      isLevelCodeVisible:true,


      // How do they look?
      slideFromDirection:'left', //left|right|top|bottom
	  
	  border: '3px solid rgba(227, 143, 21, 1)', //1px solid black
	  lineColor: 'rgba(237, 237, 237, 1)', // make same as background color to get rid of the line
	  backgroundColor: 'rgba(255, 255, 255, .87)', // white

      levelCodeColor: 'rgba(92, 28, 28, 1)', // brown
      levelNameColor: 'black',
      levelAuthorColor: 'rgba(92, 28, 28, 1)',
      timerFontColor: 'rgba(92, 28, 28, 1)',
      deathCountFontColor: 'rgba(92, 28, 28, 1)',
      timerColorOnClear: 'black',

      fontSize: '87%',
      mainFont: 'SuperMarioMaker2',
      levelCodeFont: 'SuperMarioMaker2',

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