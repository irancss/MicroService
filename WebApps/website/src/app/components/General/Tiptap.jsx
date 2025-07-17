'use client'

import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import React, { useCallback } from 'react'

const Tiptap = () => {
    const editor = useEditor({
        extensions: [StarterKit],
        content: '<p>Hello World! 🌎️</p>',
    })

    const setBold = useCallback(() => {
        editor?.chain().focus().toggleBold().run()
    }, [editor])

    const setItalic = useCallback(() => {
        editor?.chain().focus().toggleItalic().run()
    }, [editor])

    const setStrike = useCallback(() => {
        editor?.chain().focus().toggleStrike().run()
    }, [editor])

    const setHeading = useCallback((level) => {
        editor?.chain().focus().toggleHeading({ level }).run()
    }, [editor])

    const setBulletList = useCallback(() => {
        editor?.chain().focus().toggleBulletList().run()
    }, [editor])

    const setOrderedList = useCallback(() => {
        editor?.chain().focus().toggleOrderedList().run()
    }, [editor])

    const setBlockquote = useCallback(() => {
        editor?.chain().focus().toggleBlockquote().run()
    }, [editor])

    const setCodeBlock = useCallback(() => {
        editor?.chain().focus().toggleCodeBlock().run()
    }, [editor])

    if (!editor) {
        return null
    }

    return (
        <div>
            <div style={{
                display: 'flex',
                gap: 8,
                marginBottom: 8,
                padding: 8,
                borderBottom: '1px solid #ddd',
                background: '#f9f9f9'
            }}>
                <button onClick={setBold} style={{ fontWeight: editor.isActive('bold') ? 'bold' : 'normal' }}>
                    Bold
                </button>
                <button onClick={setItalic} style={{ fontStyle: editor.isActive('italic') ? 'italic' : 'normal' }}>
                    Italic
                </button>
                <button onClick={setStrike} style={{ textDecoration: editor.isActive('strike') ? 'line-through' : 'none' }}>
                    Strike
                </button>
                <button onClick={() => setHeading(1)} style={{ fontWeight: editor.isActive('heading', { level: 1 }) ? 'bold' : 'normal' }}>
                    H1
                </button>
                <button onClick={() => setHeading(2)} style={{ fontWeight: editor.isActive('heading', { level: 2 }) ? 'bold' : 'normal' }}>
                    H2
                </button>
                <button onClick={() => setHeading(3)} style={{ fontWeight: editor.isActive('heading', { level: 3 }) ? 'bold' : 'normal' }}>
                    H3
                </button>
                <button onClick={setBulletList} style={{ fontWeight: editor.isActive('bulletList') ? 'bold' : 'normal' }}>
                    Bullet List
                </button>
                <button onClick={setOrderedList} style={{ fontWeight: editor.isActive('orderedList') ? 'bold' : 'normal' }}>
                    Ordered List
                </button>
                <button onClick={setBlockquote} style={{ fontWeight: editor.isActive('blockquote') ? 'bold' : 'normal' }}>
                    Blockquote
                </button>
                <button onClick={setCodeBlock} style={{ fontWeight: editor.isActive('codeBlock') ? 'bold' : 'normal' }}>
                    Code Block
                </button>
            </div>
            <EditorContent editor={editor} />
        </div>
    )
}

export default Tiptap
