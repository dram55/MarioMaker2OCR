(function(){
   'use strict';

   var app = angular.module('dramBarApp');
   app.controller('innerFadeController', innerFadeController);
   app.directive('innerFade', innerFadeDirective);

   // innerFadeDirective
   //
   // Pass in a list of strings and they will cycle every x milliseconds
   // with a fadeIn/fadeOut transition. Similar to a slideshow.
   // 
   // The container is sized to the largest string in the given items.
   //
   // Usage:
   // <inner-fade items="ctrl.listOfStrings" interval="8000"></inner-fade>
   //
   innerFadeDirective.inject = ['$interval', '$timeout'];
   function innerFadeDirective($interval, $timeout) {
      return {
         restrict: 'E',
         templateUrl: '/drambar/innerfade.html',
         scope: {
            items: '<',
            interval: '<'
         },
         controller: innerFadeController,
         controllerAs: 'innerfade',
         bindToController: true,
         link: function innerFadeLink(scope, element) {

            // on items change
            scope.$watchCollection('innerfade.items', function() {
               if (scope.innerfade.items)
                  scope.innerfade.longestItem = scope.innerfade.items.reduce(function (a, b) { return a.length > b.length ? a : b; });
            });

            // wrap in $timeout to allow DOM to load children
            $timeout(function(){
               var children = element.find("p.inner-fade-item");
               var visibleIndex = 0;
               var maxIndex = children.length;

               // hide children by default
               angular.forEach(jQuery(children), function(value, key) {
                  var child = angular.element(value);
                  child.hide();
               });

               // immediately display first child
               jQuery(element).show();
               jQuery(children[0]).fadeTo(500, 1);
               
               // cycle through all elements - for each interval
               $interval(function() {
                  // Get next element index
                  var nextIndex = visibleIndex + 1;
                  if (nextIndex >= maxIndex)
                     nextIndex = 0;
         
                  // fade out current element
                  jQuery(children[visibleIndex]).fadeTo(500, 0, function(){
                     // display next element
                     jQuery(children[nextIndex]).fadeTo(500, 1);
                  });
         
                  // increment index
                  visibleIndex = nextIndex;
         
               }, scope.innerfade.interval); // interval
               
            }, 100); // timeout
         }
      }
   }
   function innerFadeController() {}
})();