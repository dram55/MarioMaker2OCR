(function(){
   'use strict';

   var app = angular.module('queueStatusApp', ['angular-websocket']);
   app.controller('queueStatusController',queueStatusController);
   app.factory('smmFactory', smmFactory);

   queueStatusController.inject = ['smmFactory'];
   function queueStatusController(smmFactory) {
      var ctrl = this;
      ctrl.smm = smmFactory;
   }

   smmFactory.inject = ['$websocket', '$timeout'];
   function smmFactory($websocket, $timeout) {
      var server = 'ws://localhost:3000/wss';
      var socket = $websocket(server,null,{reconnectIfNotNormalClose: true});
	  

      var status = {
         isQueueOpen: false,
         changeInProgress: null
      }

      // put a query out for the current queue status - on websocket open.
	  socket.onOpen(function(message) {
		socket.send({ action: 'queuestatusquery' });
	  });

      socket.onMessage(function(message) {
         // binary
         if (message.data instanceof Blob)
         {
            var reader = new FileReader();
            reader.onload = () => {
               handleEvent(JSON.parse(reader.result));
            };

            reader.readAsText(message.data);
         }
         // json
         else handleEvent(JSON.parse(message.data));
      });

      function handleEvent(event) {
         switch(event.type) {
            case 'queueopen':   queueStatusChange(true); break;
            case 'queueclosed':  queueStatusChange(false); break;
          }
      }

       function queueStatusChange(isOpen){
         if (isOpen !== status.isQueueOpen) {
            // delay 2 seconds
            $timeout(function() {
				status.changeInProgress = true;
			    $timeout(function() {
					status.isQueueOpen = isOpen;
					status.changeInProgress = false;
				},1500);
            }, 1);
         }
       }

      return status;
   }

})();