import {
  __commonJS
} from "./chunk-5WRI5ZAA.js";

// node_modules/react-flow/index.js
var require_react_flow = __commonJS({
  "node_modules/react-flow/index.js"(exports) {
    var Queue = function(modules) {
      var installedModules = {};
      function __webpack_require__(moduleId) {
        if (installedModules[moduleId])
          return installedModules[moduleId].exports;
        var module2 = installedModules[moduleId] = {
          /******/
          exports: {},
          /******/
          id: moduleId,
          /******/
          loaded: false
          /******/
        };
        modules[moduleId].call(module2.exports, module2, module2.exports, __webpack_require__);
        module2.loaded = true;
        return module2.exports;
      }
      __webpack_require__.m = modules;
      __webpack_require__.c = installedModules;
      __webpack_require__.p = "/__build__/";
      return __webpack_require__(0);
    }([
      /* 0 */
      /***/
      function(module2, exports2) {
        "use strict";
        var _createClass = /* @__PURE__ */ function() {
          function defineProperties(target, props) {
            for (var i = 0; i < props.length; i++) {
              var descriptor = props[i];
              descriptor.enumerable = descriptor.enumerable || false;
              descriptor.configurable = true;
              if ("value" in descriptor) descriptor.writable = true;
              Object.defineProperty(target, descriptor.key, descriptor);
            }
          }
          return function(Constructor, protoProps, staticProps) {
            if (protoProps) defineProperties(Constructor.prototype, protoProps);
            if (staticProps) defineProperties(Constructor, staticProps);
            return Constructor;
          };
        }();
        function _classCallCheck(instance, Constructor) {
          if (!(instance instanceof Constructor)) {
            throw new TypeError("Cannot call a class as a function");
          }
        }
        var Queue2 = function() {
          function Queue3() {
            _classCallCheck(this, Queue3);
            this.queue = [];
            this._queue = [];
            this.type = "LIFO";
          }
          _createClass(Queue3, [{
            key: "Queue",
            value: function Queue4(type) {
              this.type = type == "LOOP" ? "LOOP" : "LIFO";
            }
          }, {
            key: "put",
            value: function put(work) {
              this.type == "LOOP" ? (this.queue.push(work), this._queue.push(work)) : this.queue.push(work);
            }
          }, {
            key: "get",
            value: function get() {
              if (this.queue.length) {
                return this.queue.shift();
              } else {
                if (this._queue.length) {
                  this.queue = this._queue;
                }
                return false;
              }
            }
          }]);
          return Queue3;
        }();
        module2.exports = new Queue2();
      }
      /******/
    ]);
    var Store = function(modules) {
      var installedModules = {};
      function __webpack_require__(moduleId) {
        if (installedModules[moduleId])
          return installedModules[moduleId].exports;
        var module2 = installedModules[moduleId] = {
          /******/
          exports: {},
          /******/
          id: moduleId,
          /******/
          loaded: false
          /******/
        };
        modules[moduleId].call(module2.exports, module2, module2.exports, __webpack_require__);
        module2.loaded = true;
        return module2.exports;
      }
      __webpack_require__.m = modules;
      __webpack_require__.c = installedModules;
      __webpack_require__.p = "/__build__/";
      return __webpack_require__(0);
    }([
      /* 0 */
      /***/
      function(module2, exports2) {
        "use strict";
        var _createClass = /* @__PURE__ */ function() {
          function defineProperties(target, props) {
            for (var i = 0; i < props.length; i++) {
              var descriptor = props[i];
              descriptor.enumerable = descriptor.enumerable || false;
              descriptor.configurable = true;
              if ("value" in descriptor) descriptor.writable = true;
              Object.defineProperty(target, descriptor.key, descriptor);
            }
          }
          return function(Constructor, protoProps, staticProps) {
            if (protoProps) defineProperties(Constructor.prototype, protoProps);
            if (staticProps) defineProperties(Constructor, staticProps);
            return Constructor;
          };
        }();
        function _classCallCheck(instance, Constructor) {
          if (!(instance instanceof Constructor)) {
            throw new TypeError("Cannot call a class as a function");
          }
        }
        var store = function() {
          function store2() {
            _classCallCheck(this, store2);
            this.store = {};
          }
          _createClass(store2, [{
            key: "get",
            value: function get(key) {
              return this.store[key];
            }
          }, {
            key: "set",
            value: function set(key, value) {
              key && value && (this.store[key] = value);
              return key;
            }
          }, {
            key: "del",
            value: function del(key) {
              if (this.store[key]) {
                delete this.store[key];
                return key;
              } else {
                return false;
              }
            }
          }]);
          return store2;
        }();
        module2.exports = new store();
      }
      /******/
    ]);
    var Distributed = function(modules) {
      var installedModules = {};
      function __webpack_require__(moduleId) {
        if (installedModules[moduleId])
          return installedModules[moduleId].exports;
        var module2 = installedModules[moduleId] = {
          /******/
          exports: {},
          /******/
          id: moduleId,
          /******/
          loaded: false
          /******/
        };
        modules[moduleId].call(module2.exports, module2, module2.exports, __webpack_require__);
        module2.loaded = true;
        return module2.exports;
      }
      __webpack_require__.m = modules;
      __webpack_require__.c = installedModules;
      __webpack_require__.p = "/__build__/";
      return __webpack_require__(0);
    }([
      /* 0 */
      /***/
      function(module2, exports2) {
        "use strict";
        var _createClass = /* @__PURE__ */ function() {
          function defineProperties(target, props) {
            for (var i = 0; i < props.length; i++) {
              var descriptor = props[i];
              descriptor.enumerable = descriptor.enumerable || false;
              descriptor.configurable = true;
              if ("value" in descriptor) descriptor.writable = true;
              Object.defineProperty(target, descriptor.key, descriptor);
            }
          }
          return function(Constructor, protoProps, staticProps) {
            if (protoProps) defineProperties(Constructor.prototype, protoProps);
            if (staticProps) defineProperties(Constructor, staticProps);
            return Constructor;
          };
        }();
        function _classCallCheck(instance, Constructor) {
          if (!(instance instanceof Constructor)) {
            throw new TypeError("Cannot call a class as a function");
          }
        }
        var distributed = function() {
          function distributed2() {
            _classCallCheck(this, distributed2);
            this.store = {};
          }
          _createClass(distributed2, [{
            key: "findIndex",
            value: function findIndex(store, type, callback) {
              var index = -1;
              store.forEach(function(e, i) {
                if (e && e["type"] === type || e.callback === callback) {
                  return index = i;
                }
              });
              return index;
            }
          }, {
            key: "sub",
            value: function sub(type, callback) {
              if (typeof type == "string" && typeof callback == "function") {
                if (!this.store[type] || !Array.isArray(this.store[type])) {
                  this.store[type] = [{ type, callback }];
                } else if (this.findIndex(this.store[type], type, callback)) {
                  this.store[type].push({ type, callback });
                }
              }
            }
          }, {
            key: "pub",
            value: function pub(type) {
              if (typeof type == "string") {
                if (this.store[type] && Array.isArray(this.store[type])) {
                  this.store[type].forEach(function(e, i) {
                    e.callback.call(e.context, type);
                  });
                }
              }
            }
          }, {
            key: "unsub",
            value: function unsub(type, callback) {
              if (typeof type == "string") {
                if (this.store[type] && Array.isArray(this.store[type])) {
                  var i = this.findIndex(this.store[type], type, callback);
                  i >= 0 ? this.store[type].splice(i, 1) : "";
                }
              }
            }
          }]);
          return distributed2;
        }();
        module2.exports = new distributed();
      }
      /******/
    ]);
    var flow = function(modules) {
      var installedModules = {};
      function __webpack_require__(moduleId) {
        if (installedModules[moduleId])
          return installedModules[moduleId].exports;
        var module2 = installedModules[moduleId] = {
          /******/
          exports: {},
          /******/
          id: moduleId,
          /******/
          loaded: false
          /******/
        };
        modules[moduleId].call(module2.exports, module2, module2.exports, __webpack_require__);
        module2.loaded = true;
        return module2.exports;
      }
      __webpack_require__.m = modules;
      __webpack_require__.c = installedModules;
      __webpack_require__.p = "/__build__/";
      return __webpack_require__(0);
    }([
      /* 0 */
      /***/
      function(module2, exports2) {
        "use strict";
        var _createClass = /* @__PURE__ */ function() {
          function defineProperties(target, props) {
            for (var i = 0; i < props.length; i++) {
              var descriptor = props[i];
              descriptor.enumerable = descriptor.enumerable || false;
              descriptor.configurable = true;
              if ("value" in descriptor) descriptor.writable = true;
              Object.defineProperty(target, descriptor.key, descriptor);
            }
          }
          return function(Constructor, protoProps, staticProps) {
            if (protoProps) defineProperties(Constructor.prototype, protoProps);
            if (staticProps) defineProperties(Constructor, staticProps);
            return Constructor;
          };
        }();
        function _classCallCheck(instance, Constructor) {
          if (!(instance instanceof Constructor)) {
            throw new TypeError("Cannot call a class as a function");
          }
        }
        var Flow = function() {
          function Flow2() {
            _classCallCheck(this, Flow2);
            this.version = "1.0.3";
            this.actionTypes = {};
            this.storeQueue = [];
            Queue.Queue("LOOP");
          }
          _createClass(Flow2, [{
            key: "createActions",
            value: function createActions(actionCreators) {
              var name = void 0;
              var creator = void 0;
              var actionsId = (this.id++).toString(32);
              var self = this;
              var actions = {};
              for (name in actionCreators) {
                creator = actionCreators[name];
                actions[name] = /* @__PURE__ */ function(creator2, actionsId2) {
                  return function() {
                    for (var _len = arguments.length, args = Array(_len), _key = 0; _key < _len; _key++) {
                      args[_key] = arguments[_key];
                    }
                    self.__dispatch__(actionsId2, function() {
                      return creator2.apply(null, Array.prototype.slice.call(args));
                    });
                  };
                }(creator, actionsId);
              }
              return actions;
            }
          }, {
            key: "createStore",
            value: function createStore(callbacks) {
              if (!callbacks) {
                throw new Error("callbacks不能为空");
              }
              var proxyMethon = {};
              proxyMethon.get = Store.get.bind(Store);
              proxyMethon.set = Store.set.bind(Store);
              proxyMethon.sub = Distributed.sub.bind(Distributed);
              proxyMethon.unsub = Distributed.unsub.bind(Distributed);
              this.storeQueue.push({
                store: Store,
                callbacks
              });
              return proxyMethon;
            }
          }, {
            key: "combineFlow",
            value: function combineFlow(middleware) {
              typeof middleware === "function" && Queue.put(middleware);
            }
          }, {
            key: "__callback__",
            value: function __callback__(bear) {
              this.storeQueue.forEach(function(item) {
                var callback = item.callbacks[bear.type], result = void 0, changeKey = void 0;
                if (typeof callback === "function") {
                  result = callback(item.store, bear);
                  if (result !== void 0) {
                    Distributed.pub(result);
                  }
                }
              });
            }
          }, {
            key: "__dispatch__",
            value: function __dispatch__(actionsId, action) {
              var _this = this;
              var self = this, bear = action(), actionTypes = this.actionTypes, actionType = bear.type, lastId = void 0;
              if (!actionType) throw new Error("action指令不存在 \n" + JSON.stringify(payload, null, 2));
              lastId = actionTypes[actionType];
              if (!lastId) {
                actionTypes[actionType] = actionsId;
              } else if (lastId !== actionsId) {
                throw new Error('action类型 "' + actionType + '" 重复');
              }
              var cb = function cb2(bear2) {
                var result = Queue.get();
                ;
                result && result(bear2, Store.store[bear2.type], cb2.bind(_this, bear2));
                !result && _this.__callback__(bear2);
              };
              cb(bear);
            }
          }]);
          return Flow2;
        }();
        var flow2 = new Flow();
        flow2.combineFlow(function(data, store, next) {
          next();
        });
        module2.exports = { combineFlow: flow2.combineFlow.bind(flow2), createActions: flow2.createActions.bind(flow2), createStore: flow2.createStore.bind(flow2) };
      }
      /******/
    ]);
    exports.combineFlow = flow.combineFlow.bind(flow);
    exports.createActions = flow.createActions.bind(flow);
    exports.createStore = flow.createStore.bind(flow);
  }
});
export default require_react_flow();
//# sourceMappingURL=react-flow.js.map
