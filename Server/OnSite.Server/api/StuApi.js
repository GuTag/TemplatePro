const models = require('../db')
const express = require('express')
const router = express.Router()
const $sql = require('../sqlMap')
var sqlite3 = require('sqlite3').verbose()

const jsonWrite = function (res, ret) {
  if (typeof ret === 'undefined') {
    res.json({
      code: '1', msg: '操作失败'
    })
  } else {
    res.json(
      ret
    )
  }
}

// var db = new sqlite3.Database('DB.db', (err)=>{
//   if (err) throw err;
//   console.log('数据库连接')
// })

var db = new sqlite3.Database("E:\\net5.0-windows\\DB.db", (err)=>{
  if (err) throw err;
  console.log('数据库连接')
})


router.get('/GetLeftTop', (req, res) => {
  const sql = $sql.Map.SQL_GetLeftTop
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})


router.get('/GetLeftCenter', (req, res) => {
  const sql = $sql.Map.SQL_GetLeftCenter
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetLeftBottom', (req, res) => {
  const sql = $sql.Map.SQL_GetLeftBottom
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetCenterTop', (req, res) => {
  const sql = $sql.Map.SQL_GetCenterTop
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetCenterCenter', (req, res) => {
  const sql = $sql.Map.SQL_GetCenterCenter
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetCenterBottom', (req, res) => {
  const sql = $sql.Map.SQL_GetCenterBottom
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetRightTop', (req, res) => {
  const sql = $sql.Map.SQL_GetRightTop
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetRightCenter', (req, res) => {
  const sql = $sql.Map.SQL_GetRightCenter
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

router.get('/GetRightBottom', (req, res) => {
  const sql = $sql.Map.SQL_GetRightBottom
  const params = req.body
  console.log(params)
  db.all(sql,function(err,result){
    if (err) {
      console.log(err)
    }
    if (result) {
      jsonWrite(res, result)
    }
  })
})

module.exports = router
