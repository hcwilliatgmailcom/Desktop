<!DOCTYPE html>
<html>
<head>
    <title>List</title>
</head>
<body>
<?php prin_r($items)
    <table>
        <tbody>
            <?php foreach ($items as $item): ?>
                <tr>
                     <?php foreach ($item as $key => $value): ?>
                        <td><?php echo htmlspecialchars($value); ?></td>
                    <?php endforeach; ?>
                    <td>
                        <a href="/<?php echo $table; ?>/detail/<?php echo $item['id']; ?>">Detail</a>
                    </td>
                </tr>   
            <?php endforeach; ?>
        </tbody>
    </table>
    <div>
        <?php if ($page > 1): ?>
            <a href="/<?php echo $table; ?>/index/?page=<?php echo $page - 1; ?>">Previous</a>
        <?php endif; ?>

        <?php for ($i = 1; $i <= $totalPages; $i++): ?>
            <?php if ($i == $page): ?>
                <strong><?php echo $i; ?></strong>
            <?php else: ?>
                <a href="/<?php echo $table; ?>/index/?page=<?php echo $i; ?>"><?php echo $i; ?></a>
            <?php endif; ?>
        <?php endfor; ?>

        <?php if ($page < $totalPages): ?>
            <a href="/<?php echo $table; ?>/index/?page=<?php echo $page + 1; ?>">Next</a>
        <?php endif; ?>
    </div>
</body>
</html>