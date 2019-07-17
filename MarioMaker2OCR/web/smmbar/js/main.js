// Small websocket wrapper to enable autoreconnect of the socket if the program crashes
function connectToWSS(callback) {
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

var SMMBarModel = function() {
  var self = this;
  //Level Information
  this.level = {
    name: ko.observable(),
    code: ko.observable(),
    author: ko.observable(),
  }
  
  //State displayed on the bar
  this.state = {
    deaths: ko.observable(),
    cleared: ko.observable(),
    timer: ko.observable(),
  };

  //Array of the most recent events, [newest..oldest]
  this.events = ko.observableArray(),

  //Stores the timestamp indicating when the event most recently happened
  this.timestamps = {
    level: ko.observable(),
    death: ko.observable(),
    restart: ko.observable(),
    exit: ko.observable(),
    clear: ko.observable(),
  }


  //Computed value for UI use
  this.isPlaying = ko.pureComputed(function() {
    return this.level.name() !== undefined
  }, this)

  //Computed value for the UI to use
  this.state.hasCleared = ko.pureComputed(function() {
      return self.state.cleared() !== undefined;
  }, this)

  //Generates the timer string
  this.state.displayTimer = ko.pureComputed(function() {
    if(self.timestamps.level() === undefined) return "";
    if(!self.isPlaying) return "";
    
    elapsed = self.state.timer();
    hours = Math.floor(elapsed/3600)
    minutes = Math.floor((elapsed%3600)/60)
    seconds = elapsed%60
    self.state.timer(); //Force recalc on timer tick
    return hours.toString().padStart(2, "0") +":"+ minutes.toString().padStart(2, "0") +":"+ seconds.toString().padStart(2, "0");
  })

  // Just clears all the 
  this.clearState = function() {
    this.level.name(undefined);
    this.level.code(undefined);
    this.level.author(undefined);
    this.state.deaths(0);
    this.state.cleared(undefined)
    this.state.timer(0)
  }

  //called every second to update the timer
  this.timer_tick = function() {
    if(self.timestamps.level() === undefined) return;
    var elapsed = Math.floor((new Date().getTime() - self.timestamps.level().getTime())/1000)
    this.state.timer(elapsed);
    if(settings.timer.playWarning && elapsed == settings.timer.playWarningAtMinute*60) {
      document.getElementById("timerAudio").play();

    }
  }

  //All new events from the websocket pass through here and get formated
  this.newEvent = function(event) {
    eventObj = {
      timestamp: new Date(),
      type: undefined,
      data: undefined,
    }
    eventObj.type = event.type;
    if(event.type == 'level') {
      eventObj.data = event.level;
    }
    this.events.unshift(eventObj);
    while(this.events.length > 20) this.events.pop();
    this.processEvent(eventObj);
  }

  //Primary event processor with a normalized event format
  // {"type":"the event type[level, death, clear, restart, exist]",
  //  "data":{"Only used by type=level right now to share level information"}}
  this.processEvent = function(e) {
    if(this.timestamps[e.type]!==undefined) this.timestamps[e.type](new Date())
    switch(e.type) {
      case 'level':
        this.clearState();
        this.level.name(e.data.name)
        this.level.code(e.data.code)
        this.level.author(e.data.author)
        break;
      case 'restart':
        // Don't fall through to death if they just cleared.
        if(this.events.length > 1 && this.events()[1].type == 'clear') break;

        // Don't fall through to death if we had a death within the last second
        // Sometimes it'll detect the death bubble and the start over at the same time
        if(this.timestamps.death() !== undefined) {
          elapsed = new Date().getTime() - this.timestamps.death().getTime();
          if(elapsed < 1000) break;
        }

        // fallthrough to death as a restart counts as a death
      case 'death':
        if(!this.isPlaying()) {
          newLevel(settings.messages.unknown_level_name, "", "");
        }
        this.state.deaths(this.state.deaths()+1);
        break;
      case 'clear':
        this.state.cleared(true)
        break;
      case 'exit':
        this.clearState();
        break;

    }
  }
}

ko.bindingHandlers.fadeVisible = {
    init: function(element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value));
    },
    update: function(element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).slideDown('slow') : $(element).slideUp('slow');
    }
};
ko.bindingHandlers.fadeHidden = {
    init: function(element, valueAccessor) {
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value));
    },
    update: function(element, valueAccessor) {
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).slideUp('slow') : $(element).slideDown('slow');
    }
};

connectToWSS(function(event) {
  if(event['data'] === undefined) return;
  data = JSON.parse(event.data)
  if(data['type'] !== undefined) {
    app.newEvent(data);
  } else if (data['level'] !== undefined) {
    app.newEvent({
      'type': 'level',
      'level': data['level'],
    })
  }
})


var app = new SMMBarModel();
$(function() {
  app.clearState();
  ko.applyBindings(app);
})
setInterval(function() { app.timer_tick(); }, 1000)


//Useful for testing
function newLevel(name, code, author) {
  app.newEvent({
    'type': 'level',
    'level': {
      name: name,
      code: code,
      author: author,
    },
  })
}
function newEvent(evt) {
  app.newEvent({
    "type": evt
  })
}
