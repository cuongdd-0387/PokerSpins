mergeInto(LibraryManager.library, {
  ReadyToPlay: function () {
    dispatchReactUnityEvent("ReadyToPlay");
  },
  LogErrorOfGame: function (output, stack) {
    dispatchReactUnityEvent("LogErrorOfGame", Pointer_stringify(output), Pointer_stringify(stack));
  },
  EndgameResult: function (dataEndGame) {
    dispatchReactUnityEvent("EndgameResult", Pointer_stringify(dataEndGame));
  }
});