<template>
  <div>
    <div id="unity-container" class="unity-desktop">
      <canvas id="unity-canvas" @mouseover="setPause(0)" @mouseleave="setPause(1)"></canvas>
      <div id="unity-loading-bar">
        <div id="unity-logo"></div>
        <div id="unity-progress-bar-empty">
          <div id="unity-progress-bar-full"></div>
        </div>
      </div>
      <!-- <div id="unity-mobile-warning">
                WebGL builds are not supported on mobile devices.
            </div> -->
      <div id="unity-footer">
        <div id="unity-webgl-logo"></div>
        <div id="unity-fullscreen-button"></div>
        <div id="unity-build-title">TIM Demo</div>
      </div>
    </div>
  </div>
</template>

<script>
import Vue from 'vue'
import myDict from './tim-dict.js'
// console.log(myDict);

// console.log(myDict);

export const UnityEvent = new Vue()
// console.log(UnityEvent);

function timCustomLogWarn(msg) {
  if (msg.indexOf('tim') > -1) {
    //   console.log('tim: ' + msg);
    UnityEvent.$emit('tim_event', msg)
  } else {
    console.log('*************************** system warning ***************************')
    console.log(msg)
  }
}

window.console.warn = timCustomLogWarn

export default {
  name: 'timUnity',
  props: {},
  data() {
    return {
      unityInstance: null
    }
  },
  beforeMount() {},
  mounted() {},

  created() {
    // global ref register
    this.$root.$refs.TimUnity = this
  },

  methods: {
    unityEventCB(msg) {
      console.log('got msg from unity to handle : ' + msg)
      if (msg.indexOf('tim Clicked') > -1) {
        // TODO better handle msg need
        // FIXME need strip space by trim()
        var clickedObjName = msg
          .split('(')[0]
          .split('gameobject:')[1]
          .trim()
        // console.log('cleaned name on click : ' + clickedObjName);
        // console.log('cleaned name on click : ' + clickedObjName.length);
        // console.log(myDict.hasOwnProperty(clickedObjName));

        var clickedObjName1 = myDict[clickedObjName]
        var clickedObjNameCheck = myDict[clickedObjName1]
        console.log('mapped name : ' + clickedObjNameCheck)
        // custom event items
        UnityEvent.$emit('unity_event_obj_clicked', clickedObjName1)
      }
    },
    // debugBtn(){
    //     console.log("debug");
    //     if(this.unityInstance){
    //         // console.log(this.unityInstance);
    //     }
    // },

    getCameraCurrentPosition() {
      this.message('MainCamera', 'getCameraCurrentPosition', '')
    },

    sendMsgBtn(gameObject, method, param) {
      this.message(gameObject, method, param)
      // console.log(gameObject);
    },

    // test() {
    //   // this.message("MainCamera","testMultiParams",p1,p2,p3);
    //   var val = this.message('MainCamera', 'tryToGetValFromUnity', 'hello')
    //   console.log(val)
    // },
    // TODO
    setMouseDragMoveSensitivity(val) {
      this.message('MainCamera', 'SetMouseDragMoveSensitivity', val)
    },
    viewScene() {
      this.message('MainCamera', 'gotoViewScene', '')
    },
    interScene() {
      this.message('ViewMainCamera', 'gotoInterScene', '')
    },
    toggleDebugInfo(val) {
      this.message('MainCamera', 'toggleDebugInfo', val)
    },
    hideObj(name) {
      this.message('MainCamera', 'hideObj', name)
    },
    unHideObj(name) {
      this.message('MainCamera', 'unHideObj', name)
    },
    hideCurrentSelected() {
      this.message('MainCamera', 'hideCurrentSelected', '')
    },
    selectedAsFocus() {
      this.message('MainCamera', 'selectedAsFocus', '')
    },
    resetScene(val) {
      console.log('tim unity reset scene with : ' + val)
      this.message('MainCamera', 'ResetView', '')
    },

    setPause(val) {
      console.log('hover ' + val)
      this.message('MainCamera', 'SetCameraDisable', val)
      this.message('GameObject', 'setPause', val)
    },
    // interface to unity
    message(gameObject, method, param) {
      if (param === null) {
        param = ''
      }
      if (this.unityInstance !== null) {
        this.unityInstance.SendMessage(gameObject, method, param)
      } else {
        console.error("vue-unity-webgl: you've sent a message to the Unity content, but it wasn\t instantiated yet.")
      }
    }
  },

  mounted() {
    // path to unity build files local or url
    var buildUrl = 'Build'
    var loaderUrl = buildUrl + '/Build.loader.js'
    // loaderUrl = "https://...";

    var notCompressedConfig = {
      dataUrl: buildUrl + '/Build.data',
      frameworkUrl: buildUrl + '/Build.framework.js',
      codeUrl: buildUrl + '/Build.wasm',
      streamingAssetsUrl: 'StreamingAssets',
      companyName: 'DefaultCompany',
      productName: 'New Unity Project',
      productVersion: '0.1'
    }

    // config for compressed
    var compressedConfig = {
      dataUrl: buildUrl + '/Build.data.unityweb',
      frameworkUrl: buildUrl + '/Build.framework.js.unityweb',
      codeUrl: buildUrl + '/Build.wasm.unityweb',
      streamingAssetsUrl: 'StreamingAssets',
      companyName: 'DefaultCompany',
      productName: 'TIM Demo',
      productVersion: '0.1'
    }

    // use compresses or not
    var config = notCompressedConfig

    var container = document.querySelector('#unity-container')
    var canvas = document.querySelector('#unity-canvas')

    // TODO progress bar or unity canvas css not work yet
    var loadingBar = document.querySelector('#unity-loading-bar')
    var progressBarFull = document.querySelector('#unity-progress-bar-full')
    var fullscreenButton = document.querySelector('#unity-fullscreen-button')
    var mobileWarning = document.querySelector('#unity-mobile-warning')

    if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
      // TODO leave mobile for now
      // container.className = "unity-mobile";
      // config.devicePixelRatio = 1;
      // mobileWarning.style.display = "none";
    } else {
      // TODO maybe receive resultion when init vue component,keep ratio
      canvas.style.width = '960px'
      canvas.style.height = '600px'
    }
    loadingBar.style.display = 'block'

    var script = document.createElement('script')
    script.src = loaderUrl
    script.onload = () => {
      createUnityInstance(canvas, config, progress => {
        progressBarFull.style.width = 100 * progress + '%'
      })
        .then(unityInstance => {
          this.unityInstance = unityInstance
          loadingBar.style.display = 'none'
          fullscreenButton.onclick = () => {
            unityInstance.SetFullscreen(1)
          }
        })
        .catch(message => {
          alert(message)
        })
    }
    document.body.appendChild(script)

    // into next event handle by event
    UnityEvent.$on('tim_event', msg => {
      this.unityEventCB(msg)
    })
  }
}
</script>

<style scoped>
@import '../../assets/unity/TemplateData/style.css';
</style>
