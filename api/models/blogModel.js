const pool = require('../db')

const getPublished = async () => {
    const result = await pool.query(
        `SELECT * FROM blogs WHERE published = true ORDER BY created_at DESC`
    )
    return result.rows
}

const getAll = async () => {
    const result = await pool.query(
        'SELECT * FROM blogs ORDER BY created_at DESC'
    )
    return result.rows
}

const getByID = async (id) => {
    const result = await pool.query(
        'SELECT * FROM blogs WHERE id = $1',
        [id]
    )
    return result.rows[0]

}

const create = async (user_id, title, content, published) => {
    const result = await pool.query(
        'INSERT INTO blogs (user_id, title, content, published) VALUES ($1, $2, $3, $4) RETURNING *'
        [user_id, title, content, published]
    )
    return result.rows[0]
}

const update = async (id, title, content, published) => {
    const result = await pool.query(
        'UPDATE blogs SET title = $1, content = $2, published = $3, updated_at = NOW() WHERE id = $4 RETURNING *',
        [title, content, published, id]
    )
    return result.rows[0]
}

const remove = async (id) => {
    await pool.query('DELETE FROM blogs WHERE id = $1', [id])
}

module.exports = {getPublished , getAll, getByID, create, update, remove}