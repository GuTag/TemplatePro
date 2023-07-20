// sqlMap.js  sql语句
// import AMessage from '../views/AMessage/index.vue'



const sqlMap = {
  Map: {

    SQL_GetLeftTop: 'SELECT * FROM Left_Top ORDER BY ID DESC LIMIT 0,1',
    SQL_GetLeftCenter: 'SELECT * FROM Left_Center ORDER BY ID DESC LIMIT 0,1',
    SQL_GetLeftBottom: 'SELECT * FROM Left_Bottom ORDER BY ID DESC LIMIT 0,20',

    SQL_GetCenterTop: 'SELECT * FROM Center_Top ORDER BY ID DESC LIMIT 0,1',
    SQL_GetCenterCenter: 'SELECT * FROM Center_Center ORDER BY ID DESC LIMIT 0,1',
    SQL_GetCenterBottom: 'SELECT * FROM Center_Bottom ORDER BY ID DESC LIMIT 0,20',

    SQL_GetRightTop: 'SELECT * FROM Right_Top ORDER BY ID DESC LIMIT 0,1',
    SQL_GetRightCenter: 'SELECT * FROM Right_Center ORDER BY ID DESC LIMIT 0,1',
    SQL_GetRightBottom: 'SELECT * FROM Right_Bottom ORDER BY ID DESC LIMIT 0,20',

  }
  
}
module.exports = sqlMap









