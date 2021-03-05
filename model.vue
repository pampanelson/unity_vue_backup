<template>
  <div>
    <timUnity />
    <!-- <div id="unity_info_test">{{unity_info}}</div> -->
    <!-- <button @click="selectedAsFocus()">当前所选对象为焦点</button> -->
    <!-- <button @click="hideCurrentSelected()">隐藏当前选定对象</button>
        <button @click="resetScene()">重置</button> -->
    <el-button @click="hideCurrentSelected()" type="danger" style="float:left; margin-left:3%; margin-top:10px;"
      >隐藏当前选定对象</el-button
    >
    <el-button @click="resetScene()" type="warning" style="float:left; margin-left:1%; margin-top:10px;"
      >重置</el-button
    >
    <el-button @click="debugInfo(0)" type="primary" style="float:left; margin-left:1%; margin-top:10px;"
      >隐藏debug信息</el-button
    >
    <el-button @click="debugInfo(1)" type="primary" style="float:left; margin-left:1%; margin-top:10px;"
      >显示debug信息</el-button
    >
    <el-button @click="viewScene()" type="primary" style="float:left; margin-left:1%; margin-top:10px;"
      >动画漫游场景</el-button
    >
    <el-button @click="interScene()" type="primary" style="float:left; margin-left:1%; margin-top:10px;"
      >交互场景</el-button
    >
    <el-table
      v-if="diceng"
      :data="tableData"
      border
      style="width: 96%; float:left; margin-left:2%; margin-top:670px; margin-bottom:20px; position: relative"
    >
      <el-table-column align="center" prop="gneissName" label="岩土体名称"> </el-table-column>
      <el-table-column align="center" prop="rockDescription" label="岩性描述" width="280"> </el-table-column>
      <el-table-column align="center" prop="rockLevel" label="围岩级别"> </el-table-column>
      <el-table-column align="center" prop="unitWeight" label="容重γ（kN/m3）"> </el-table-column>
      <el-table-column align="center" prop="saturationStrength" label="饱和抗压强度Rc(MPa)"> </el-table-column>
      <el-table-column align="center" prop="modulDeformation" label="变形模量E（GPa）"> </el-table-column>
      <el-table-column align="center" prop="poissonRatio" label="泊松比μ"> </el-table-column>
      <el-table-column align="center" prop="frictionAngle" label="内摩擦角ψ"> </el-table-column>
      <el-table-column align="center" prop="cohesion" label="粘聚力c（MPa）"> </el-table-column>
      <el-table-column align="center" prop="carryingCapacity" label=" 承载力基本容许值（kPa）"> </el-table-column>
      <el-table-column align="center" prop="earthRockLevel" label="土石工程分级"> </el-table-column>
    </el-table>
    <el-table
      v-if="chenqi"
      :data="tableData1"
      border
      style="width: 96%; float:left; margin-left:2%; margin-top:670px; margin-bottom:20px; position: relative"
    >
      <el-table-column align="center" prop="stationCategory" label="设计分类"> </el-table-column>
      <el-table-column align="center" prop="stationPart" label="设计部位"> </el-table-column>
      <el-table-column align="center" prop="stationStartNumber" label="开始桩号"> </el-table-column>
      <el-table-column align="center" prop="stationEndNumber" label="结束桩号"> </el-table-column>
      <el-table-column align="center" prop="stationStartMile" label="开始里程"> </el-table-column>
      <el-table-column align="center" prop="stationEndMile" label="结束里程"> </el-table-column>
      <el-table-column align="center" prop="stationLevel" label="围岩级别"> </el-table-column>
      <el-table-column align="center" prop="stationStruct" label="结构类型"> </el-table-column>
      <el-table-column align="center" prop="structMethod" label="施工方法"> </el-table-column>
    </el-table>
    <el-table
      v-if="qita"
      :data="tableData2"
      border
      style="width: 96%; float:left; margin-left:2%; margin-top:670px; margin-bottom:20px; position: relative"
    >
      <el-table-column align="center" prop="stationStruct" label="结构类型"> </el-table-column>
      <el-table-column align="center" prop="componentName" label="构件名称"> </el-table-column>
      <el-table-column align="center" prop="componentSubType" label="构件子类"> </el-table-column>
      <el-table-column align="center" prop="componentPropertyName" label="构件属性名"> </el-table-column>
      <el-table-column align="center" prop="componentPropertyValue" label="构件属性值"> </el-table-column>
    </el-table>
  </div>
</template>

<script>
import timUnity from './tim-unity' // 导入组件
import { UnityEvent } from './tim-unity' // 导入事件对象

export default {
  name: 'model',
  components: {
    timUnity // 在html中显示unity
  },
  props: {},
  data() {
    return {
      unity_info: '',
      datathree: {
        modelName: ''
      },
      diceng: false,
      tableData: [],
      chenqi: false,
      tableData1: [],
      qita: false,
      tableData2: []
    }
  },
  beforeMount() {},
  methods: {
    setMouseDragMoveSensitivity(val) {
      this.$root.$refs.TimUnity.setMouseDragMoveSensitivity()
    },
    viewScene() {
      this.$root.$refs.TimUnity.viewScene()
    },
    interScene() {
      this.$root.$refs.TimUnity.interScene()
    },
    debugInfo(val) {
      this.$root.$refs.TimUnity.toggleDebugInfo(val)
    },
    hideCurrentSelected() {
      this.$root.$refs.TimUnity.hideCurrentSelected() //调用unity函数
    },
    resetScene() {
      this.$root.$refs.TimUnity.resetScene('hello') //带参数调用unity函数
    },

    // 定义unity事件回调函数，带返回值
    async handleUnityObjClicked(name) {
      console.log(name, 11111111)
      this.unity_info = name
      this.datathree.modelName = name
      const data = await this.$store.dispatch('station/list1', this.datathree)
      if (data.geologyInformation !== undefined) {
        this.tableData = [data.geologyInformation]
        this.diceng = true
        this.chenqi = false
        this.qita = false
      } else if (data.station !== undefined) {
        this.tableData1 = [data.station]
        this.diceng = false
        this.chenqi = true
        this.qita = false
      } else if (data.componentInfos !== undefined) {
        this.tableData2 = data.componentInfos
        this.diceng = false
        this.chenqi = false
        this.qita = true
      }
    }
  },
  mounted() {
    // 在mounted阶段为事件注册回调函数
    UnityEvent.$on('unity_event_obj_clicked', name => {
      this.handleUnityObjClicked(name)
      console.log(3123123213123)
    })
  }
}
</script>

<style scoped></style>
