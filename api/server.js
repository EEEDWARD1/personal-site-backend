const express = require('express')
const helmet = require('helmet')
const cors = require('cors')
const rateLimit = require('express-rate-limit')

const app = express()
const port = process.env.PORT || 8080
//middleware
app.use(helmet())
/*app.use(cors({
    origin: 'https://eduardteodor.co.uk'
}))*/
app.use(rateLimit({
    windowMs: 15 * 60 * 1000,
    max: 100
}))
app.use(express.json())

app.use('/auth', require('./routes/auth'))
app.use('/blogs', require('./routes/blogs'))
app.use('/admin', require('./routes/admin'))

app.get('/', (req, res) => {
    res.status(200).json({
        status: 'ok',
        uptime: process.uptime(),
        timestamp: new Date().toISOString()
    })
})

app.listen(port, () => console.log(`Server started on port ${port}`))
