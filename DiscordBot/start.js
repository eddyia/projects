const express = require("express")
const app = express()
const port = 1080
var id; 

app.use(express.static('QuestionBank'))

app.use('css', express.static(__dirname + '/QuestionBank/resources/css'))

console.log(__dirname + './QuestionBank/resources/css')

app.use('js', express.static(__dirname + '/QuestionBank/resources/js'))

app.use('fonts', express.static(__dirname + '/QuestionBank/resources/fonts'))

app.use('images', express.static(__dirname + '/QuestionBank/resources/images'))

app.get('', (req, res) => {
  res.sendFile(__dirname + '/QuestionBank/GeneratedPapers/GeneratedPaper.html')
})

app.listen(port, () => console.info('Listening on port ${port} '))