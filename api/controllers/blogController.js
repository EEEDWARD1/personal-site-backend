const blogModel = require(`../models/blogModel`)

const getPublishedBlogs = async (req, res) => {
    try {
        const blogs = await blogModel.getPublished()
        res.json(blogs)
    } catch (err){
        res.sendStatus(500)
    }
}

const getAllBlogs = async (req, res) => {
    try {
        const blogs = await blogModel.getAll()
        res.json(blogs)
    } catch (err){
        res.sendStatus(500)
    }
}

const createBlog = async (req, res) => {
    try{
        const { title, content, published } = req.body
        const blog = await blogModel.create(req.user.id, title, content, published ?? false)
        res.status(201).json(blog)
    } catch (err) {
        res.sendStatus(500)
    }
}
const updateBlog = async (req, res) => {
    try {
        const { title, content, published } = req.body
        const blog = await blogModel.update(req.params.id, title, content, published)
        res.json(blog)
    } catch (err) {
        res.sendStatus(500)
    }
}

const deleteBlog = async (req, res) => {
    try {
        await blogModel.remove(req.params.id)
        res.sendStatus(204)
    } catch (err) {
        res.sendStatus(500)
    }
}

module.exports = { getPublishedBlogs, getAllBlogs, createBlog, updateBlog, deleteBlog }