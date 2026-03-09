const pool = require ('../db')

const findByEmail = async (email) => {
    const result = await pool.query(
        'SELECT * FROM users WHERE email = $1',
        [email]
    )
    return result.row[0]
}

module.exports = {findByEmail}