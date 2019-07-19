(function(){
   'use strict';

   var app = angular.module('dramBarApp',['angular-websocket']);
   app.controller('dramBarController',dramBarController);
   app.factory('smmFactory', smmFactory);

   dramBarController.inject = ['smmFactory', 'dramBarSettings'];
   function dramBarController(smmFactory, dramBarSettings) {
      var ctrl = this;
      ctrl.smm = smmFactory;
      ctrl.settings = dramBarSettings;
   }

   smmFactory.inject = ['$websocket', '$timeout', '$interval'];
   function smmFactory($websocket, $timeout, $interval) {
      var server = 'ws://localhost:3000/wss';
      var socket = $websocket(server,null,{reconnectIfNotNormalClose: true});

      var levelStartTime;

      var status = {
         deathCount:0,
         changeInProgress:true,
         level: {},
         levelTimer: "",
         deathInProgress:false
      }

      // Timer functions - time on current level
      function startTimer(){
         levelStartTime = new Date();
         $interval(timerTick, 1000);
      }
      function pauseTimer(){
         $interval.cancel(stop);
      }
      function resetTimer(){
         $interval.cancel(stop);
         status.levelTimer = "";
      }
      function timerTick(){
         var currentTime = new Date().getTime();
         var totalSeconds = (currentTime - levelStartTime) / 1000;
         var hours = Math.floor(totalSeconds / 3600);
         var minutes = (Math.floor(totalSeconds / 60) % 60);
         var seconds = Math.floor(totalSeconds % 60);
         status.levelTimer = "";
         if (hours>0) status.levelTimer += hours.toString().padStart(2, '0') + ":";
         status.levelTimer += minutes.toString().padStart(2, '0');
         status.levelTimer += ":" + seconds.toString().padStart(2, '0');
      }
      $interval(timerTick, 1000);

      socket.onMessage(function(message) {
         var event = JSON.parse(message.data);

         switch(event.type) {
            case 'death':   death(event); break;
            case 'restart': death(event); break;
            case 'exit':    exit(event);    break;
            case 'clear':   clear(event);   break;
            case 'level':   newLevel(event);   break;
          }

          if (event.level) newLevel(event);
      });

      function death() {
         console.log('death');
         if (!status.deathInProgress) {
            status.deathInProgress = true;
            $timeout(function() {
               status.deathCount = status.deathCount + 1;
               status.deathInProgress = false;
            }, 500);
         }
       }
       function restart(){
         console.log('restart');
         status.deathCount = status.deathCount + 1;
       }
       function exit(){
         console.log('exit');
         status.changeInProgress = true;
         status.deathCount = 0;
         pauseTimer();
       }
       function clear(data){
         console.log('clear');
         pauseTimer();
       }

       function newLevel(data){
         if (data.level.code !== status.level.code) {
            startTimer();
            if (status.changeInProgress) {
               displayNewLevel();
            }
            else {
               // delay 2 seconds - for animation to remove current level
               status.changeInProgress = true;
               $timeout(function() {
                  displayNewLevel();
               }, 1500);
            }
         }

         function displayNewLevel(){
            status.changeInProgress = false;
            status.level = data.level;
            deathCount: 0;
            levelStartTime = new Date().getTime();
         }
       }

      return status;
   }

})();