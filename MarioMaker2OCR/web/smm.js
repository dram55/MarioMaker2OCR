// A basic cookie wrapper that automatically sets the path attribute
function setCookie(name, val,) {
  document.cookie = name + "=" + val + ";path="+window.location.pathname;
}

// Parses out the desired cookie
function getCookie(cname) {
  var name = cname + "=";
  var decodedCookie = decodeURIComponent(document.cookie);
  var ca = decodedCookie.split(';');
  for(var i = 0; i <ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') {
          c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
          return c.substring(name.length, c.length);
      }
  }
  return "";
}

// Small websocket wrapper to enable autoreconnect of the socket if the program crashes
function connectToSMM(callback) {
  var ws = new WebSocket('ws://localhost:'+location.port+'/wss');
  ws.onmessage = callback
  ws.onclose = function(e) {
    console.log('Socket is closed. Reconnect will be attempted in 3 second.', e.reason);
    setTimeout(function() {
      connectToSMM(callback);
    }, 3000);
  };
  ws.onerror = function(err) {
    console.error('Socket encountered error: ', err.message, 'Closing socket');
    ws.close();
  };
}
