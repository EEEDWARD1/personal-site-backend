const bcrypt = require('bcrypt')
const jwt = require(`jsonwebtoken`)
const userModel = require(`../models/userModel`)

const login = async (req, res) => {
    try {
        const {email, password} = req.body

        const user = await userModel.findByEmail(email)
        if (!user) return res.sendStatus(401)
        
        const valid = await bcrypt.compare(password, user.password_hash)
        if (!valid) return res.sendStatus(401)

        const token = jwt.sign(
            { id: user.id, email: user.email},
            process.env.JWT_SECRET,
            {expiresIn: '8h'}
        )

        res.json({ token })
    } catch (err) {
        console.error(err)
        res.sendStatus(500)
    }
}