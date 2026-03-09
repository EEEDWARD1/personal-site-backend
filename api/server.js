const express = require('express')
const app = express()
const port = process.env.PORT || 8080

app.use(express.json())

app.use('/auth', require('./routes/auth'))
app.use('/blogs', require('./routes/blogs'))
app.use('/admin', require('./routes/admin'))

app.listen(port, () => console.log(`Server started on port ${port}`))