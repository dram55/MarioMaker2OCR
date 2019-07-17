(function(){
   'use strict';

   var app = angular.module('dramBarApp',['angular-websocket']);
   app.controller('dramBarController',dramBarController);
   app.factory('smmFactory', smmFactory);

   dramBarController.inject = ['smmFactory'];
   function dramBarController(smmFactory) {
      var ctrl = this;
      ctrl.smm = smmFactory;
   }

   smmFactory.inject = ['$websocket', '$timeout'];
   function smmFactory($websocket, $timeout) {
      var server = 'ws://localhost:3000/wss';
      var socket = $websocket(server);

      var status = {
         deathCount:0,
         changeInProgress:true,
         level: {}
      }

      socket.onMessage(function(message) {
         var event = JSON.parse(message.data);

         switch(event.type) {
            case 'death':   death(event);   break;
            case 'restart': restart(event); break;
            case 'exit':    exit(event);    break;
            case 'clear':   clear(event);   break;
            case 'level':   newLevel(event);   break;
          }

          if (event.level) newLevel(event);
      });

      function death() {
         console.log('death');
         status.deathCount = status.deathCount + 1;
       }
       function restart(){
         console.log('restart');
         status.deathCount = status.deathCount + 1;
       }
       function exit(){
         console.log('exit');
         status.changeInProgress = true;
       }
       function clear(data){
         console.log('clear');
       }

       function newLevel(data){
         if (data.level.code !== status.level.code) {
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
         }
       }

      return status;
   }

})();