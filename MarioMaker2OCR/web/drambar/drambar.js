(function () {
      'use strict';

      var app = angular.module('dramBarApp', ['angular-websocket']);
      app.controller('dramBarController', dramBarController);
      app.factory('dramBarFactory', dramBarFactory);

      dramBarController.inject = ['dramBarFactory', 'dramBarSettings'];

      function dramBarController(dramBarFactory, dramBarSettings) {
         var ctrl = this;
         ctrl.state = dramBarFactory;
         ctrl.settings = dramBarSettings;
      }

      dramBarFactory.inject = ['$websocket', '$timeout', '$interval', 'dramBarSettings'];
      function dramBarFactory($websocket, $timeout, $interval, dramBarSettings) {

         var server = 'ws://localhost:' + location.port + '/wss';
         var socket = $websocket(server, null, {reconnectIfNotNormalClose: true});

         var levelStartTime;
         var timerRunning;

         var state = {
            deathCount: 0,
            hideCurrentLevel: true,
            level: {},
            levelTimer: "",
            deathInProgress: false,
            levelCleared: false
         };

         // Timer functions - time on current level
         function startTimer() {
            levelStartTime = new Date();
            timerRunning = true;
            timerTick(); // force a tick to reset clock
         }

         function pauseTimer() {
            timerRunning = false;
         }

         function resumeTimer() {
            timerRunning = true;
         }

         function timerTick() {
            if (!timerRunning) {
               return;
            }
            var currentTime = new Date().getTime();
            var totalSeconds = (currentTime - levelStartTime) / 1000;
            var hours = Math.floor(totalSeconds / 3600);
            var minutes = (Math.floor(totalSeconds / 60) % 60);
            var seconds = Math.floor(totalSeconds % 60);
            state.levelTimer = "";
            if (hours > 0) state.levelTimer += hours.toString().padStart(2, '0') + ":";
            state.levelTimer += minutes.toString().padStart(2, '0');
            state.levelTimer += ":" + seconds.toString().padStart(2, '0');
            if (dramBarSettings.playTimerWarning && (dramBarSettings.playTimerWarningAt * 60) == Math.floor(totalSeconds)) {
               document.getElementById("timerAudio").play();
            }
         }
         $interval(timerTick, 1000);

         socket.onMessage(function (message) {
            var event = JSON.parse(message.data);

            switch (event.type) {
               case 'death': death(event); break;
               case 'restart': restart(event); break;
               case 'exit': exit(event); break;
               case 'gameover': gameover(event); break;
               case 'clear': clear(event); break;
               case 'level': newLevel(event); break;
            }

            if (event.level) newLevel(event);
         });

         function death() {
            console.log('death');
            if (!state.deathInProgress) {
               state.deathInProgress = true;
               $timeout(function () {
                  state.deathCount = state.deathCount + 1;
                  state.deathInProgress = false;
               }, 500);
            }
         }

         function restart() {
            // if you are starting over after clear - then refresh timer
            if (state.levelCleared) {
               displayNewLevel(state.level);
            } else {
               death();
            }
         }

         function exit() {
            console.log('exit');
            pauseTimer();

            state.hideCurrentLevel = true;
         }

         function clear(data) {
            console.log('clear');
            pauseTimer();
            state.levelCleared = true;
         }

         function gameover(data) {
            console.log('gameover');
            pauseTimer();
         }

         function newLevel(data) {
            // Resume timer if paused - and level isn't cleared from a previous quit or gameover
            if(!timerRunning && !state.levelCleared && data.level.code === state.level.code) {
               if (state.hideCurrentLevel)
                  state.hideCurrentLevel = false;

               resumeTimer();
            }

            // Only process new level if the level data is hidden OR if the level code has changed.
            else if (data.level.code !== state.level.code || state.hideCurrentLevel) {
               // If panel is hidden, display immediately
               if (state.hideCurrentLevel) {
                  displayNewLevel(data.level);
               } else {
                  // delay 1.5 seconds - to allow for scroll animation
                  state.hideCurrentLevel = true;
                  $timeout(function () {
                     displayNewLevel(data.level);
                  }, 1500);
               }
            }
         }

         function displayNewLevel(newLevel) {
            state.hideCurrentLevel = false;
            state.level = newLevel;
            state.deathCount  ;

            startTimer();
            state.levelCleared = false;
         }
         return state;
      }
})();